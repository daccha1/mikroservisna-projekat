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

		public MQClient()
		{
			certificationService = new();

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

			// certification created queue

			await _channel.QueueDeclareAsync(
				queue: certificationCreatedQueue,
				durable: false,
				exclusive: false,
				autoDelete: false
			);

			await _channel.QueueBindAsync(
				queue: certificationCreatedQueue,
				exchange: choreographyExchange,
				routingKey: certificationCreatedRouting
			);

			// certification email fail
			await _channel.QueueDeclareAsync(
				queue: certificationEmailFailQueue,
				durable: false,
				exclusive: false,
				autoDelete: false
			);

			await _channel.QueueBindAsync(
				queue: certificationEmailFailQueue,
				exchange: choreographyExchange,
				routingKey: certificationEmailFailRouting
			);



			// certification failed queue
			await _channel.QueueDeclareAsync(
				queue: certificationFinalFailQueue,
				durable: false,
				exclusive: false,
				autoDelete: false
			);

			await _channel.QueueBindAsync(
				queue: certificationFinalFailQueue,
				exchange: choreographyExchange,
				routingKey: certificationFinalFailRouting
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
					// <<< PROVERI >>>
					var msg = await certificationService.HandleCertificationServiceFail(evt);
					await SendMessage("final-fail-certification", JsonSerializer.Serialize<CertificationFailed>(msg));
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

			// 
			var certificationFailConsumer = new AsyncEventingBasicConsumer(_channel);

			certificationFailConsumer.ReceivedAsync += async (_, ea) =>
			{
				var body = Encoding.UTF8.GetString(ea.Body.Span);
				var evt = JsonSerializer.Deserialize<CertificationCompleted>(body);
				
				if(evt.State == CertificationState.Cancelled)
				{		
					var msg = await certificationService.HandleEmailFailed(evt);

					await SendMessage(certificationFinalFailRouting, JsonSerializer.Serialize<CertificationFailed>(msg));
				}
				
				await _channel.BasicAckAsync(
							deliveryTag: ea.DeliveryTag,
							multiple: false
						);
			};

			await _channel.BasicConsumeAsync(
					queue: certificationEmailFailQueue,
					autoAck: false,
					consumer: certificationFailConsumer
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

		public string certificationCreatedQueue = "events.certifications.certification-created";
		public string certificationCreatedRouting = "certification-created";

		public string certificationEmailFailQueue = "events.email.certification-email-fail";
		public string certificationEmailFailRouting = "certification-email-fail";
		
		public string certificationFinalFailQueue = "events.certification.certification-final-fail";
		public string certificationFinalFailRouting = "final-fail-certification";

	}
}
