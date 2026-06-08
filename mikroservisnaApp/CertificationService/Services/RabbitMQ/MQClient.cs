using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Common.Saga_Contracts.Choreography;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CertificationService.Services.RabbitMQ
{
	public class MQClient : IDisposable
	{
		public static MQClient Instance 
		{
			get
			{
				if (instance == null) instance = new MQClient();
				return instance;
			}
		}
		private static MQClient instance;

		private ConnectionFactory factory;
		private IChannel _channel;
		private IConnection _connection;
		private CertificationService certificationService;

		public async Task StartClient()
		{
			certificationService = new();

			factory = new ConnectionFactory()
			{
				HostName = "localhost",
				UserName = "guest",
				Password = "guest"
			};
			_connection = await factory.CreateConnectionAsync();
			_channel = await _connection.CreateChannelAsync();

			await _channel.ExchangeDeclareAsync(
				exchange: choreographyExchange,
				type: ExchangeType.Direct,
				durable: false,
				autoDelete: false
			);

			await _channel.QueueDeclareAsync(
				queue: certificationRequestQueue,
				durable: false,
				exclusive: false,
				autoDelete: false
			);

			await _channel.QueueBindAsync(
				queue: certificationRequestQueue,
				exchange: choreographyExchange,
				routingKey: certificationRequestRouting
			);

			var certificationRequestConsumer = new AsyncEventingBasicConsumer(_channel);

			certificationRequestConsumer.ReceivedAsync += async (_, ea) =>
			{
				var body = Encoding.UTF8.GetString(ea.Body.Span);
				var evt = JsonSerializer.Deserialize<CertificationRequested>(body);

				int result = await certificationService.HandleCertificationRequest(evt);

				if(result == 1)
				{

					await certificationService.HandleCreatedCertificate(evt.CorrelationId);
					
				}
				else
				{
					// kompenzacija i vrati nazad poruku
				}

				await _channel.BasicAckAsync(
							deliveryTag: ea.DeliveryTag,
							multiple: false
						);
			};
			
			await _channel.BasicConsumeAsync(
					queue: certificationRequestQueue,
					autoAck: false,
					consumer: certificationRequestConsumer
				);
		}

		public async Task SendMessage(string routingkey, string payload)
		{
			byte[] byteBody = Encoding.UTF8.GetBytes(payload);

			await _channel.BasicPublishAsync(
					exchange: choreographyExchange,
					routingKey: routingkey,
					mandatory: true,
					body: byteBody
				);
		}

		public void Dispose()
		{
			_connection?.Dispose();
		}



		public string choreographyExchange = "choreography-exchange";

		public string certificationRequestQueue = "events.posetilac.certification-requested";
		public string certificationRequestRouting = "certification-requested";
	}
}
