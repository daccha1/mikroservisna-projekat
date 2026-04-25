using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;

namespace mikroservisnaApp.MQ_Container
{

	public interface IMQClient
	{
		public Task EnsureStarted();
		public Task SendMessageAsync(string jsonBody, string exchangeName, string routingKeyString, string replyToString= "");

		public Task ReceiveMessageAsync();

		public event EventHandler<string> OdgovorPrimljen;
	}

	

	public class MQClient : IMQClient, IAsyncDisposable
	{
		IConnection? connection;
		IChannel? publishChannel;
		AsyncEventingBasicConsumer? locationConsumer;
		AsyncEventingBasicConsumer? organizerConsumer;
		
		private readonly ConcurrentDictionary<string, string> _pendingRequests = new();
		
		public event EventHandler<string>? OdgovorPrimljen;

		// email
		string emailExchangeName = "events.event.eventsExchange";
		string emailQueueName = "events.event.publishQueue";
		string emailRoutingKey = "event-publish-key";

		// location
		string locationExchangeName = "events.location.locationExchange";
		string locationQueueName = "events.location.publishQueue";
		string locationRoutingKey = "location-publish-key";

		string locationConsumeQueue = "events.location.consumeQueue";
		string locationConsumeKey = "location-consume-key";

		// organizator
		string organizatorExchangeName = "events.organizer.organizerExchange";
		string organizatorQueueName = "events.organizer.organizerQueue";
		string organizatorRoutingKey = "organizer-publish-key";

		string organizatorConsumeQueue = "events.organizer.consumeQueue";
		string organizatorConsumeKey = "organizer-consume-key";

		public async Task SendMessageAsync(string jsonBody, string exchangeName, string routingKeyString, string replyToString="")
		{
			await EnsureStarted();
			

			if(connection == null || publishChannel == null)
			{
				return;
			}

			try
			{
				Console.WriteLine(">>>>POKRENUTA METODA SLANJA PORUKE!");
				string correlationId = Guid.NewGuid().ToString("N");
				_pendingRequests[correlationId] = jsonBody;

				byte[] byteBody = Encoding.UTF8.GetBytes(jsonBody);
				

				var properties = new BasicProperties
				{
					Persistent = true,
					ContentType = "application/json",
					ReplyTo = replyToString,
					CorrelationId = correlationId
				};


				await publishChannel.BasicPublishAsync(
						exchange: exchangeName,
						routingKey: routingKeyString,
						body: byteBody,
						basicProperties: properties,
						mandatory: true
					);
				Console.WriteLine(">>>>POSLATA PORUKA!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($" >>>>>>>> Greska: {ex.Message}");
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
					routingKey: locationRoutingKey
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
					routingKey: locationConsumeKey
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


			await ReceiveMessageAsync();

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

		public async Task ReceiveMessageAsync()
		{
			if(locationConsumer != null && organizerConsumer != null)
			{
				return;
			}

			locationConsumer = new AsyncEventingBasicConsumer(publishChannel);
			Console.WriteLine(">>> locationConsumer kreiran!");
			locationConsumer.ReceivedAsync += async (_, ea) =>
			{
				Console.WriteLine(">>> USAO U locationConsumer handler!");
				string body = Encoding.UTF8.GetString(ea.Body.ToArray());
				Console.WriteLine($">>> Body: {body}");
				Console.WriteLine($">>> CorrelationId: {ea.BasicProperties.CorrelationId}");
				if (!body.IsNullOrEmpty())
				{
					var correlationId = ea.BasicProperties.CorrelationId;										// OVDE OBRATI PAZNJU
					if (string.IsNullOrWhiteSpace(correlationId) || !_pendingRequests.TryRemove(correlationId, out var request))
					{
						return;
					}

					await publishChannel.BasicAckAsync(
							ea.DeliveryTag,
							multiple: false
						);

					OdgovorPrimljen?.Invoke(this, body);
				}
			};
			await publishChannel.BasicConsumeAsync(locationConsumeQueue, autoAck: false, locationConsumer);
			Console.WriteLine($">>> Consumer registrovan na queue: {locationConsumeQueue}");

			Console.WriteLine($">>> EnsureStarted ZAVRSEN! Connection: {connection.IsOpen}, Channel: {publishChannel.IsOpen}");

			//organizerConsumer = new AsyncEventingBasicConsumer(publishChannel);
			//organizerConsumer.ReceivedAsync += async (_, ea) =>
			//{
			//	string body = Encoding.UTF8.GetString(ea.Body.ToArray());
			//	if (Convert.ToInt32(body) != 0)
			//	{
			//		var correlationId = ea.BasicProperties.CorrelationId;
			//		if (string.IsNullOrWhiteSpace(correlationId) || !_pendingRequests.TryRemove(correlationId, out var request))
			//		{
			//			return;
			//		}

			//		await publishChannel.BasicAckAsync(
			//				ea.DeliveryTag,
			//				multiple: false
			//			);

			//		OdgovorPrimljen?.Invoke(this, body);
			//	}
			//};
			//await publishChannel.BasicConsumeAsync(organizatorConsumeQueue, autoAck: false, organizerConsumer);

		}
	}
}
