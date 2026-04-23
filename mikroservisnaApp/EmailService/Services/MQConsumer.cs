using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Common.StrucniDogadjajDTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace EmailService.Services
{
	public class MQConsumer
	{
		IConnection? connection;
		IChannel? channel;

		// exhange, queue
		string exchangeName = "events.event.eventsExchange";
		string queueName = "events.event.publishQueue";
		public async Task StartClient()
		{
			if (connection != null && channel != null)
			{
				return;
			}

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
				try
				{
					byte[] body = ea.Body.ToArray();
					string jsonString = Encoding.UTF8.GetString(body);
					StrucniDogadjajRequestDTO? strucniDogadjaj = JsonSerializer.Deserialize<StrucniDogadjajRequestDTO>(jsonString);

					await EmailSenderClient.Instance.SendMessage(strucniDogadjaj);

					Console.WriteLine(strucniDogadjaj);

					await channel.BasicAckAsync(ea.DeliveryTag, false);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);

					
					await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
				}
			};
			await channel.BasicConsumeAsync(queueName, autoAck: false, consumer);
			Console.WriteLine("Consumer is listening!");
		}
	}
}
