using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;

namespace mikroservisnaApp.MQ_Container
{

	public interface IMQPublisher
	{
		public Task EnsureStarted();
		public Task SendMessage(string jsonBody);
	}

	public class MQPublisher : IMQPublisher, IAsyncDisposable
	{
		IConnection? connection;
		IChannel? publishChannel;

		// email
		string emailExchangeName = "events.event.eventsExchange";
		string emailQueueName = "events.event.publishQueue";
		string emailRoutingKey = "event-publish-key";

		// location
		string locationExchangeName = "events.location.locationExchange";
		string locationQueueName = "events.location.locationQueue";
		string locationRoutingKey = "location-publish-key";

		// organizator
		string organizatorExchangeName = "events.organizer.organizerExchange";
		string organizatorQueueName = "events.organizer.organizerQueue";
		string organizatorRoutingKey = "organizer-publish-key";

		public async Task SendMessage(string jsonBody)
		{
			await EnsureStarted();

			if(connection == null || publishChannel == null)
			{
				return;
			}

			try
			{
				byte[] byteBody = Encoding.UTF8.GetBytes(jsonBody);

				var properties = new BasicProperties
				{
					Persistent = true,
					ContentType = "application/json"
				};


				await publishChannel.BasicPublishAsync(
						exchange: emailExchangeName,
						routingKey: "event-publish-key",
						body: byteBody,
						basicProperties: properties,
						mandatory: true
					);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($" >>>>>>>> Greska: {ex.Message}");
			}

		}

		public async Task EnsureStarted()
		{

			if(connection != null && publishChannel != null)
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

			#region emailExchange
			await publishChannel.ExchangeDeclareAsync(
					exchange: emailExchangeName,
					type: ExchangeType.Direct,
					durable: false,
					autoDelete: false
				);
			await publishChannel.QueueDeclareAsync(
					queue: emailQueueName,
					durable:false,
					exclusive:false,
					autoDelete:false
				);
			await publishChannel.QueueBindAsync(
					queue: emailQueueName,
					exchange: emailExchangeName,
					routingKey: emailRoutingKey
				);
			#endregion

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
			#endregion

			#region organizerExchange
			await publishChannel.ExchangeDeclareAsync(
					exchange: organizatorExchangeName,
					type: ExchangeType.Direct,
					durable: false,
					autoDelete: false
				);
			await publishChannel.QueueDeclareAsync(
					queue: organizatorQueueName,
					durable: false,
					exclusive: false,
					autoDelete: false
				);
			await publishChannel.QueueBindAsync(
					queue: organizatorQueueName,
					exchange: organizatorExchangeName,
					routingKey: organizatorRoutingKey
				);
			#endregion



		}

		public async ValueTask DisposeAsync()
		{
			if (publishChannel is not null)
			{
				await publishChannel.DisposeAsync();
			}

			if (connection is not null)
			{
				await connection.DisposeAsync();
			}
		}
	}
}
