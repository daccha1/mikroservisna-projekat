	using Common.Saga_Contracts;
using Common.Saga_Contracts.Choreography;
using Microsoft.EntityFrameworkCore;
using PosetilacAPI.Data;
using PosetilacAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
	using System.Text.Json;
	using System.Text.Json.Serialization;
using System.Threading.Channels;


	namespace PosetilacAPI.MQ_Container
	{

		public interface IMQClient
		{
			public Task EnsureStarted();
			public Task SendMessage(PosetilacCreated evt);
			public Task SendMessage(CertificationRequested evt);
		}

		public class MQClient : IMQClient
		{
			public ConnectionFactory factory;
			public IConnection connection;
			public IChannel channel;

			public string exchangeName = "saga-exchange";

			public string pubPosetilacCreated = "events.orch.pos-creation";
			public string pubPosetilacRouting = "create-posetilac";

			public string posetilacServiceConsumeQueue = "events.posetilac.transaction-consume-queue";
			public string posetilacServiceConsumeRouting = "transaction-final-feedback";


			public string choreographyExchange = "choreography-exchange";

			public string certificationRequestQueue = "events.posetilac.certification-requested";
			public string certificationRequestRouting = "certification-requested";

			public string certificationCompletedQueue = "events.email.certification-completed";
			public string certificationCompletedRouting = "certification-completed";

			public string certificationFinalFailQueue = "events.certification.certification-final-fail";
			public string certificationFinalFailRouting = "final-fail-certification";

		private IServiceScopeFactory _scopeFactory;

			public MQClient(IServiceScopeFactory scopeFactory)
			{
				_scopeFactory = scopeFactory;
			}

			public async Task SendMessage(PosetilacCreated evt)
			{
				string jsonString = JsonSerializer.Serialize<PosetilacCreated>(evt);
				byte[] byteBody = Encoding.UTF8.GetBytes(jsonString);

				var basicProps = new BasicProperties
				{
					CorrelationId = evt.CorrelationId.ToString(),
					Persistent = true
				};

				await channel.BasicPublishAsync(
						exchange: exchangeName,
						routingKey: pubPosetilacRouting,
						basicProperties:basicProps,
						body: byteBody,
						mandatory: true
					);
			}
			public async Task EnsureStarted()
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

				// definisanje svih queues i exchanges

				await channel.ExchangeDeclareAsync(
						exchange: exchangeName,
						type: ExchangeType.Direct,
						durable: false,
						autoDelete: false
					);

				await channel.QueueDeclareAsync(
						queue: pubPosetilacCreated,
						durable: false,
						exclusive: false,
						autoDelete: false
					);

				await channel.QueueBindAsync(
						queue: pubPosetilacCreated,
						exchange: exchangeName,
						routingKey: pubPosetilacRouting
					);

				// final transaction state queue

				await channel.QueueDeclareAsync(
					queue: posetilacServiceConsumeQueue,
					durable: false,
					exclusive: false,
					autoDelete: false
				);

				await channel.QueueBindAsync(
						queue: posetilacServiceConsumeQueue,
						exchange: exchangeName,
						routingKey: posetilacServiceConsumeRouting
					);

				// --- SAGA CHOREOGRAPHY ---
				await channel.ExchangeDeclareAsync(
					exchange: choreographyExchange,
					type: ExchangeType.Direct,
					durable: false,
					autoDelete: false
				);

				await channel.QueueDeclareAsync(
					queue: certificationRequestQueue,
					durable: false,
					exclusive: false,
					autoDelete: false
				);
				await channel.QueueBindAsync(
					queue: certificationRequestQueue,
					exchange: choreographyExchange,
					routingKey: certificationRequestRouting
				);

				await channel.QueueDeclareAsync(
					queue: certificationCompletedQueue,
					durable: false,
					exclusive: false,
					autoDelete: false
				);
				await channel.QueueBindAsync(
					queue: certificationCompletedQueue,
					exchange: choreographyExchange,
					routingKey: certificationCompletedRouting
				);

				await channel.QueueDeclareAsync(
					queue: certificationFinalFailQueue,
					durable: false,
					exclusive: false,
					autoDelete: false
				);
				await channel.QueueBindAsync(
					queue: certificationFinalFailQueue,
					exchange: choreographyExchange,
					routingKey: certificationFinalFailRouting
				);

			var consumer = new AsyncEventingBasicConsumer(channel);

				consumer.ReceivedAsync += async (_, ea) =>
				{
					using var scope = _scopeFactory.CreateScope();
					var db = scope.ServiceProvider.GetService<PosetilacDbContext>();

					var jsonString = Encoding.UTF8.GetString(ea.Body.Span);
					TransactionFinalState? tfs = JsonSerializer.Deserialize<TransactionFinalState>(jsonString);

					//SagaResultOutboxMessage msg = new()
					//{
					//	CorrelationId = tfs.CorrelationId,
					//	FinalState = (tfs.TranscationStatus == FinalTransactionState.Successful ? State.Success : State.Fail),
					//	OutboxState = OutboxState.ForProcessing
					//};

					//await db.SagaResultOutbox.AddAsync(msg); *** Sto mi je ovo trebalo?

					if(tfs.TranscationStatus == FinalTransactionState.Failed)
					{
						var deletionPosetilac = await db.Posetioci.Where(p => p.CorrelationId == tfs.CorrelationId).FirstOrDefaultAsync();

						db.Posetioci.Remove(deletionPosetilac);
					}


					await db.SaveChangesAsync();

					await channel.BasicAckAsync(
							deliveryTag: ea.DeliveryTag,
							multiple: false
						);
				};

				await channel.BasicConsumeAsync(
					queue: posetilacServiceConsumeQueue,
					autoAck: false,
					consumer
				);


			// consumer koji osluskuje queue za sertifikaciju
			var certificationConsumer = new AsyncEventingBasicConsumer(channel);

			certificationConsumer.ReceivedAsync += async (_, ea) =>
			{
				using var scope = _scopeFactory.CreateScope();
				var db = scope.ServiceProvider.GetService<PosetilacDbContext>();

				var jsonString = Encoding.UTF8.GetString(ea.Body.Span);
				CertificationCompleted? msg = JsonSerializer.Deserialize<CertificationCompleted>(jsonString);

				var visitor = await db.Posetioci.Where(visitor => visitor.CorrelationId == msg.CorrelationId).FirstOrDefaultAsync();

				if(msg.State == CertificationState.Sucessful)
				{
					visitor.Certificate = Certification.Certified;
				}
				else
				{
					visitor.Certificate = Certification.Cancelled;
				}

				db.Posetioci.Update(visitor);
				await db.SaveChangesAsync();
				await channel.BasicAckAsync(
						deliveryTag: ea.DeliveryTag,
						multiple: false
					);
			};

			await channel.BasicConsumeAsync(
				queue: certificationCompletedQueue,
				autoAck: false,
				certificationConsumer
			);



			var failConsumer = new AsyncEventingBasicConsumer(channel);

			failConsumer.ReceivedAsync += async (_, ea) =>
			{
				using var scope = _scopeFactory.CreateScope();
				var db = scope.ServiceProvider.GetService<PosetilacDbContext>();

				var jsonString = Encoding.UTF8.GetString(ea.Body.Span);
				CertificationFailed? msg = JsonSerializer.Deserialize<CertificationFailed>(jsonString);

				var visitor = await db.Posetioci.Where(visitor => visitor.CorrelationId == msg.CorrelationId).FirstOrDefaultAsync();

				if (msg.FailType == FailType.EmailFail)
				{
					visitor.Certificate = Certification.Cancelled;
				}
				else
				{
					visitor.Certificate = Certification.Cancelled;
				}

				db.Posetioci.Update(visitor);
				await db.SaveChangesAsync();
				await channel.BasicAckAsync(
						deliveryTag: ea.DeliveryTag,
						multiple: false
					);
			};

			await channel.BasicConsumeAsync(
				queue: certificationFinalFailQueue,
				autoAck: false,
				failConsumer
			);

		}

			public async Task SendMessage(CertificationRequested evt)
			{
				string jsonString = JsonSerializer.Serialize<CertificationRequested>(evt);
				byte[] byteBody = Encoding.UTF8.GetBytes(jsonString);

				var basicProps = new BasicProperties
				{
					Persistent = true
				};

				await channel.BasicPublishAsync(
						exchange: choreographyExchange,
						routingKey: certificationRequestRouting,
						basicProperties: basicProps,
						body: byteBody,
						mandatory: true
				);
			}
		}
	}
