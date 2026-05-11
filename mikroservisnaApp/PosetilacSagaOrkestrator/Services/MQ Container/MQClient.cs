using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace PosetilacSagaOrkestrator.Services.MQ_Container
{
	public class MQClient : IDisposable
	{
		public ConnectionFactory factory;
		private IChannel _channel;
		private IConnection _connection;

		public void Dispose()
		{
			_connection?.Dispose();
		}

		public MQClient()
		{
			factory = new ConnectionFactory()
			{
				HostName = "localhost",
				UserName = "guest",
				Password = "guest"
			};
			_connection = factory.CreateConnectionAsync().GetAwaiter().GetResult(); 
			_channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
		}

		public async Task StartClient()
		{
			await _channel.ExchangeDeclareAsync(
						exchange: exchangeName,
						type: ExchangeType.Direct,
						durable: false,
						autoDelete: false
					);

			await _channel.QueueDeclareAsync(
					queue: pubPosetilacCreated,
					durable: false,
					exclusive: false,
					autoDelete: false
				);

			await _channel.QueueBindAsync(
					queue: pubPosetilacCreated,
					exchange: exchangeName,
					routingKey: pubPosetilacRouting
				);

			// publish create-gift
			await _channel.QueueDeclareAsync(
					queue: pubGiftPosetilac,
					durable: false,
					autoDelete: false,
					exclusive: false
				);

			await _channel.QueueBindAsync(
					queue: pubGiftPosetilac,
					exchange: exchangeName,
					routingKey: pubGiftPosetilacRouting
				);

			// consume sa gift i email-a

			await _channel.QueueDeclareAsync(
					queue: orchConsumeQueue,
					durable: false,
					exclusive: false,
					autoDelete: false
				);

			await _channel.QueueBindAsync(
					queue: orchConsumeQueue,
					exchange: exchangeName,
					routingKey: orchConsumeRouting
				);

			// email queue za notify-ing posetilaca
			await _channel.QueueDeclareAsync(
					queue: emailServiceQueue,
					durable: false,
					exclusive: false,
					autoDelete: false
				);
			await _channel.QueueBindAsync(
					queue: emailServiceQueue,
					exchange: exchangeName,
					routingKey: emailServiceRouting
				);

		}


		public async Task Publish(string routingKeyString, string payload)
		{
			byte[] byteBody = Encoding.UTF8.GetBytes(payload);

			await _channel.BasicPublishAsync(
					exchange: exchangeName,
					routingKey: routingKeyString,
					mandatory: true,
					body: byteBody
				);
		}

		public async Task Subscribe<T>(string queueName, Func<T, Task> handler)
		{
			
			var consumer = new AsyncEventingBasicConsumer(_channel);

			consumer.ReceivedAsync += async (sender, args) =>
			{
				try
				{
					var body = args.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);

					var data = System.Text.Json.JsonSerializer.Deserialize<T>(message);

					if (data != null)
						await handler(data);

					await _channel.BasicAckAsync(args.DeliveryTag, false);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[SAGA] Error consuming message: {ex.Message}");
					await _channel.BasicNackAsync(args.DeliveryTag, false, true);
				}
			};

			await _channel.BasicConsumeAsync(queueName, false, consumer);
		}




		private string exchangeName = "saga-exchange";

		private string pubPosetilacCreated = "events.orch.pos-creation";
		private string pubPosetilacRouting = "create-posetilac";

		private string pubGiftPosetilac = "events.posetilac.create-gift";
		private string pubGiftPosetilacRouting = "create-gift";

		public string orchConsumeQueue = "events.orch.consume-queue";
		public string orchConsumeRouting = "gift-created";

		public string emailServiceQueue = "events.email.notify-posetilac";
		public string emailServiceRouting = "notify-posetilac";

	}
}
