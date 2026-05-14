using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Common;
using Common.Saga_Contracts;
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

		// exch za saga orkestrator
		private string sagaExchangeName = "saga-exchange";

		public string emailServiceQueue = "events.email.notify-posetilac";
		public string emailServiceRouting = "notify-posetilac";

		// queue na koji saljemo mejlove >>> "saga-exchange";
		public string orchConsumeQueue = "events.orch.consume-queue";
		public string orchConsumeRouting = "notification-sent";

		private IServiceScopeFactory _scopeFactory;
		
		public MQConsumer(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
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

					// DLQ
					await channel.QueueDeclareAsync(
							queue: "events.event.x-dead-queue",
							durable: false,
							exclusive: false,
							cancellationToken: stoppingToken
						);
					await channel.QueueBindAsync(
							queue: "events.event.x-dead-queue",
							exchange: exchangeName,
							routingKey: "dlq-route",
							cancellationToken: stoppingToken
						);

					// saga queue & exchange

					await channel.ExchangeDeclareAsync(
							exchange: sagaExchangeName,
							type: ExchangeType.Direct,
							durable: false,
							autoDelete: false
						);
					await channel.QueueDeclareAsync(
							queue: emailServiceQueue,
							durable: false,
							exclusive: false,
							autoDelete: false
						);
					await channel.QueueBindAsync(
							queue: emailServiceQueue,
							exchange: sagaExchangeName,
							routingKey: emailServiceRouting
						);

					// publish notifikacije nazad orkestratoru

					await channel.QueueDeclareAsync(
							queue: orchConsumeQueue,
							durable: false,
							exclusive: false,
							autoDelete: false
						);
					await channel.QueueBindAsync(
							queue: orchConsumeQueue,
							exchange: sagaExchangeName,
							routingKey: orchConsumeRouting
						);


					// CONSUMER KOJI OSLUSKUJE KREIRANJE NOVOG EVENTA (event <=> strucni dogadjaj)
					var consumerEventCreated = new AsyncEventingBasicConsumer(channel);
					consumerEventCreated.ReceivedAsync += async (_, ea) =>
					{
						await HandleEventMessageAsync(ea, stoppingToken);
					};
					await channel.BasicConsumeAsync(queueName, autoAck: false, consumerEventCreated);


					// CONSUMER KOJI OSLUSKUJE SAGA EVENT: Slanje notifikacije posetiocu
					var consumerPosetilacNotification = new AsyncEventingBasicConsumer(channel);
					consumerPosetilacNotification.ReceivedAsync += async (_, ea) =>
					{
						try
						{
							await HandlePosetilacNotification(ea, stoppingToken);
						}
						catch (Exception) {}
					};
					await channel.BasicConsumeAsync(emailServiceQueue, autoAck: false, consumerPosetilacNotification);


					Console.WriteLine("Consumer is listening...");
					await Task.Delay(Timeout.Infinite, stoppingToken);
				}
				catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
				{
					break;
				}
				catch (Exception ex)
				{
					Console.WriteLine("MQ restart in 5s: " + ex.Message);
					await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
				}
			}
		}

		
		private async Task HandlePosetilacNotification(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
		{
			using var scope = _scopeFactory.CreateScope();
			var db = scope.ServiceProvider.GetService<EmailServiceDbContext>();

			// ea mora da se konvertuje u objekat
			byte[] body = ea.Body.ToArray();
			var bodyString = Encoding.UTF8.GetString(body);
			NotifyPosetilac? notification = JsonSerializer.Deserialize<NotifyPosetilac>(bodyString);

			if (notification == null) return;

			var exist = await db.ProcessedMessages.Where(pm => pm.CorrelationId == notification.CorrelationId).FirstOrDefaultAsync();

			if(exist != null)
			{
				Console.WriteLine("Vec postoji mejl kojim je obavesten dati klijent.");
				Console.WriteLine("----- OTKAZIVANJE PORUKE -----");
				await channel.BasicAckAsync(ea.DeliveryTag, false);
				return;
			}


			try
			{
				await EmailSenderClient.Instance.SendMessage(notification);

				NotifyOrchestratorEvent notifyOrch = new()
				{
					EventType = EventType.NotificationEvent,
					CorrelationId = notification.CorrelationId,
					NotificationStatus = NotificationStatus.Sent
				};

				byte[] successNotification = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<NotifyOrchestratorEvent>(notifyOrch));

				await channel.BasicPublishAsync(
						exchange: sagaExchangeName,
						routingKey: orchConsumeRouting,
						body: successNotification
					);

				var processedMessage = new ProcessedMessage()
				{
					CorrelationId = notification.CorrelationId,
					ProcessedTime = DateTime.UtcNow,
					EventId = $"{notification.CorrelationId}"
				};

				Console.WriteLine($"Sacuvan mejl za CorrelationID: {notification.CorrelationId}");

				await db.ProcessedMessages.AddAsync(processedMessage);
				await db.SaveChangesAsync();
			}
			catch (Exception e)
			{
				// nije uspelo
				Console.WriteLine(e.Message);
				NotifyOrchestratorEvent failedMailNotification = new()
				{
					EventType = EventType.NotificationEvent,
					CorrelationId = notification.CorrelationId,
					NotificationStatus = NotificationStatus.Failed
				};

				var jsonString = JsonSerializer.Serialize<NotifyOrchestratorEvent>(failedMailNotification);
				byte[] failedNotification = Encoding.UTF8.GetBytes(jsonString);

				await channel.BasicPublishAsync(
						exchange: sagaExchangeName,
						routingKey: orchConsumeRouting,
						body: failedNotification
				);

			}
			finally
			{
				await channel.BasicAckAsync(ea.DeliveryTag, false);
			}
		}

		// metoda koja salje mejl kada se kreira novi event (strucni dogadjaj)
		private async Task HandleEventMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
		{
			try
			{
				await RateLimit(stoppingToken);
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

				if(message == null || string.IsNullOrEmpty(mailObject.Naziv) || mailObject.Naziv.Length < 5)
				{
					await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
					throw new Exception($"Poruka nije validna. Poslata je na dead letter queue {DateTime.UtcNow}");
				}

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

				// ovde treba da je sibne na DLQ
				await channel.BasicPublishAsync(
						exchange: exchangeName,
						routingKey: "dlq-route",
						mandatory: true,
						body: ea.Body
					);
			};
		}

		private DateTime _windowStart = DateTime.MinValue;
		private int _processedThroughWindow;

		private TimeSpan RateLimitWindow = TimeSpan.FromMinutes(1);
		private int MaxMessagesPerWindow = 10;

		public async Task RateLimit(CancellationToken cancellationToken)
		{
			var currentMoment = DateTime.UtcNow;
			
			//      ako je default value           u trenutku uzimanja poruke, prosao minut
			if(_windowStart == DateTime.MinValue || currentMoment - _windowStart >= RateLimitWindow)
			{
				_windowStart = currentMoment;
				_processedThroughWindow = 0;
			}

			if(_processedThroughWindow < MaxMessagesPerWindow)
			{
				_processedThroughWindow++;
				return;
			}

			var windowEndsAt = _windowStart + RateLimitWindow;
			var waitTime = windowEndsAt > currentMoment ? windowEndsAt - currentMoment : TimeSpan.Zero;

			if(waitTime > TimeSpan.Zero)
			{
				Console.WriteLine($"Rate limit prekoracen. Cekanje: {waitTime.TotalSeconds}s");
				await Task.Delay(waitTime, cancellationToken);
			}

			_windowStart = DateTime.UtcNow;
			_processedThroughWindow = 1;

		}

	}
}


