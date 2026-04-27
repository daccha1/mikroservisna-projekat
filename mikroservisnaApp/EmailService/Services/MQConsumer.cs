using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Common;
using Common.StrucniDogadjajDTO;
using EmailService.Data;
using EmailService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;



namespace EmailService.Services
{
	public class MQConsumer : BackgroundService
	{
		IConnection? connection;
		IChannel? channel;

		// exhange, queue
		string exchangeName = "events.event.eventsExchange";
		string queueName = "events.event.publishQueue";

		private IServiceScopeFactory _scopeFactory;

		public MQConsumer(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		public async Task StartClient()
		{
			
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Console.WriteLine("Pokrenut je servis!");
			EmailSenderClient.Instance.StartClient();

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var factory = new ConnectionFactory()
					{
						HostName = "localhost",
						UserName = "guest",
						Password = "guest"
					};

					connection = await factory.CreateConnectionAsync();
					channel = await connection.CreateChannelAsync();

					await channel.ExchangeDeclareAsync(
							exchange: exchangeName,
							type: ExchangeType.Direct,
							durable: false,
							autoDelete: false
						);
					await channel.QueueDeclareAsync(
							queue: queueName,
							durable: false,
							exclusive: false,
							autoDelete: false
						);
					await channel.QueueBindAsync(
							queue: queueName,
							exchange: exchangeName,
							routingKey: "event-publish-key"
						);

					var consumer = new AsyncEventingBasicConsumer(channel);
					consumer.ReceivedAsync += async (_, ea) =>
					{
						await HandleMessageAsync(ea, stoppingToken);
					};

					await channel.BasicConsumeAsync(queueName, autoAck: false, consumer);
					Console.WriteLine("Consumer is listening!");

					await Task.Delay(Timeout.Infinite, stoppingToken);
				}
				catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
				{
					break;
				}
				catch (Exception ex)
				{
					Console.WriteLine("MQ consumer restart in 5s: " + ex.Message);
					await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
				}
			}
		}

		private async Task HandleMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
		{
			try
			{
				Console.WriteLine("Handler triggered!");
				using var scope = _scopeFactory.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<EmailServiceDbContext>();

				var existingMessage = await db.ProcessedMessages.AnyAsync(pm => pm.EventId == ea.BasicProperties.MessageId);

				if(existingMessage)
				{
					Console.WriteLine("Vec postoji obradjena poruka sa tim ID-em.");
					await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
					return;
				}

				Console.WriteLine("Stigla poruka");
				byte[] body = ea.Body.ToArray();
				string jsonString = Encoding.UTF8.GetString(body);
				OutboxMessage message = JsonSerializer.Deserialize<OutboxMessage>(jsonString);

				StrucniDogadjajRequestDTO mailObject = JsonSerializer.Deserialize<StrucniDogadjajRequestDTO>(message.Payload);

				await EmailSenderClient.Instance.SendMessage(mailObject);

				ProcessedMessage msg = new()
				{
					EventId = ea.BasicProperties.MessageId,
					ProcessedTime = DateTime.UtcNow
				};
				Console.WriteLine("Poruka sacuvana");
				await db.ProcessedMessages.AddAsync(msg, stoppingToken);
				await db.SaveChangesAsync(stoppingToken);

				
				Console.WriteLine(mailObject);

				await channel.BasicAckAsync(ea.DeliveryTag, false);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
			};
		}
	}
}


