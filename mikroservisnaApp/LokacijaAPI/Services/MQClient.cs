using LokacijaAPI.Data;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace LokacijaAPI.Services
{

	public interface IMQClient
	{
		public Task EnsureStarted();
	
	}

	public class MQClient : IMQClient
	{
		IConnection? connection;
		IChannel? publishChannel;
		AsyncEventingBasicConsumer? consumer;
		IServiceScopeFactory scopeFactory;


		string locationExchangeName = "events.location.locationExchange";
		string locationQueueName = "events.location.publishQueue";
		string locationRoutingKey = "location-publish-key";

		string locationConsumeQueue = "events.location.consumeQueue";
		string locationConsumeKey = "location-consume-key";


		public MQClient(IServiceScopeFactory scope)
		{
			this.scopeFactory = scope;
		}

		public bool ProveriLokaciju(int id)
		{
			using var scope = scopeFactory.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<LokacijaDbContext>();

			var lokacija = dbContext.Lokacije.Where(l => l.Id == id).FirstOrDefault();

			if(lokacija == null)
			{
				return false;
			}
			return true;

		}

		public async Task EnsureStarted()
		{

			if (connection != null && publishChannel != null)
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
			publishChannel = await connection.CreateChannelAsync();

			#region locationExchange
			await publishChannel.ExchangeDeclareAsync(
					exchange: locationExchangeName,
					type: ExchangeType.Direct,
					durable: false,
					autoDelete: false
				);
			await publishChannel.QueueDeclareAsync(
					queue: locationQueueName,
					durable: false,
					exclusive: false,
					autoDelete: false
				);
			await publishChannel.QueueBindAsync(
					queue: locationQueueName,
					exchange: locationExchangeName,
					routingKey: "location-publish-key"
				);

			await publishChannel.QueueDeclareAsync(
					queue: locationConsumeQueue,
					durable: false,
					exclusive: false,
					autoDelete: false
				);

			await publishChannel.QueueBindAsync(
					queue: locationConsumeQueue,
					exchange: locationExchangeName,
					routingKey: "location-consume-key"
				);

			consumer = new AsyncEventingBasicConsumer(publishChannel);

			consumer.ReceivedAsync += async (_, ea) =>
			{
				Console.WriteLine("STIGLA PORUKA");
				string payload = Encoding.UTF8.GetString(ea.Body.ToArray());
				Console.WriteLine("PORUKA JE: " + payload);
				if (payload.IsNullOrEmpty())
				{
					// error
				}
				int idLokacije = Convert.ToInt32(payload);
				string odgovor = "false";
				bool doesExist = ProveriLokaciju(idLokacije);
				
				if (doesExist)
				{
					odgovor = "true";
				};

				byte[] bodyResponse = Encoding.UTF8.GetBytes(odgovor);

				string correlationId = ea.BasicProperties.CorrelationId;

				var props = new BasicProperties
				{
					CorrelationId = correlationId,
					Persistent = true,
				};

				Console.WriteLine($">>> ReplyTo vrednost: '{ea.BasicProperties.ReplyTo}'");
				Console.WriteLine($">>> CorrelationId vrednost: '{ea.BasicProperties.CorrelationId}'");
				Console.WriteLine($">>> Odgovor koji saljem: '{odgovor}'");

				await publishChannel.BasicPublishAsync(
						locationExchangeName,
						routingKey: ea.BasicProperties.ReplyTo,
						body: bodyResponse,
						basicProperties: props,
						mandatory: false
				);
				Console.WriteLine(">>> BasicPublish pozvan!");

				await publishChannel.BasicAckAsync(ea.DeliveryTag, false);
				
			};
			await publishChannel.BasicConsumeAsync(
					locationQueueName,
					false,
					consumer
				);
			#endregion

		
			//await ReceiveMessageAsync();

		}


	}
}
