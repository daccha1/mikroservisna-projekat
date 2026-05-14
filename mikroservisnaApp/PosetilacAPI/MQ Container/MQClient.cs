	using Common.Saga_Contracts;
using PosetilacAPI.Data;
using PosetilacAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
	using System.Text.Json;
	using System.Text.Json.Serialization;


	namespace PosetilacAPI.MQ_Container
	{

		public interface IMQClient
		{
			public Task EnsureStarted();
			public Task SendMessage(PosetilacCreated evt);
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

				var consumer = new AsyncEventingBasicConsumer(channel);

				consumer.ReceivedAsync += async (_, ea) =>
				{
					using var scope = _scopeFactory.CreateScope();
					var db = scope.ServiceProvider.GetService<PosetilacDbContext>();

					var jsonString = Encoding.UTF8.GetString(ea.Body.Span);
					TransactionFinalState? tfs = JsonSerializer.Deserialize<TransactionFinalState>(jsonString);

					SagaResultOutboxMessage msg = new()
					{
						CorrelationId = tfs.CorrelationId,
						FinalState = (tfs.TranscationStatus == FinalTransactionState.Successful ? State.Success : State.Fail),
						OutboxState = OutboxState.ForProcessing
					};

					await db.SagaResultOutbox.AddAsync(msg);
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
				
			}


		}
	}
