	using Common.Saga_Contracts;
	using RabbitMQ.Client;
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


			//public string subGiftQueue = "events.orch.gift.send";

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
				
			}
		}
	}
