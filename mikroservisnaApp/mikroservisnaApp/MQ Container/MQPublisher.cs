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

		// exhange, queue
		string exchangeName = "events.event.eventsExchange";
		string queueName = "events.event.publishQueue";

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
						exchange: exchangeName,
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

			await publishChannel.ExchangeDeclareAsync(
					exchange: exchangeName,
					type: ExchangeType.Direct,
					durable: false,
					autoDelete: false
				);
			await publishChannel.QueueDeclareAsync(
					queue: queueName,
					durable:false,
					exclusive:false,
					autoDelete:false
				);
			await publishChannel.QueueBindAsync(
					queue: queueName,
					exchange: exchangeName,
					routingKey: "event-publish-key"
				);

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
