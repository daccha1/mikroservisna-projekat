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
		public Task SendMessageAsync(string jsonBody, string exchangeName, string routingKeyString, CancellationToken cancellationToken = default , string replyToString= "");

		public Task ReceiveMessageAsync();

		public event EventHandler<string> OdgovorPrimljenLocation;
		public event EventHandler<string> OdgovorPrimljenOrganizator;

    }

	

	public class MQClient : IMQClient, IAsyncDisposable
	{
		IConnection? connection;
		IChannel? publishChannel;
		AsyncEventingBasicConsumer? locationConsumer;
		AsyncEventingBasicConsumer? organizerConsumer;
		
		private readonly ConcurrentDictionary<string, string> _pendingRequest = new();
        private readonly ConcurrentDictionary<string, string> _pendingRequestOrganizator = new();


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

		public async Task SendMessageAsync(string jsonBody, string exchangeName, string routingKeyString, CancellationToken cancellation = default, string replyToString="")
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
				_pendingRequest[correlationId] = jsonBody;

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

            // consume queue
            await publishChannel.ExchangeDeclareAsync(
                    exchange: organizatorExchangeName,
                    type: ExchangeType.Direct,
                    durable: false,
                    autoDelete: false
                );
            await publishChannel.QueueDeclareAsync(
                    queue: organizatorConsumeQueue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false
                );
            await publishChannel.QueueBindAsync(
                    queue: organizatorConsumeQueue,
                    exchange: organizatorExchangeName,
                    routingKey: organizatorConsumeKey
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

        public event EventHandler<string>? OdgovorPrimljenLocation;
        public event EventHandler<string>? OdgovorPrimljenOrganizator;
        public async Task ReceiveMessageAsync()
		{
			if(locationConsumer != null)
			{
				return;
			}

			locationConsumer = new AsyncEventingBasicConsumer(publishChannel);
			Console.WriteLine(">>> locationConsumer kreiran!");
			locationConsumer.ReceivedAsync += async (_, ea) =>
			{
				string body = Encoding.UTF8.GetString(ea.Body.ToArray());

				if (!body.IsNullOrEmpty())
				{
					var correlationId = ea.BasicProperties.CorrelationId;										// OVDE OBRATI PAZNJU
					if (string.IsNullOrWhiteSpace(correlationId) || !_pendingRequest.TryRemove(correlationId, out var request))
					{
						return;
					}

					await publishChannel.BasicAckAsync(
							ea.DeliveryTag,
							multiple: false
						);

					OdgovorPrimljenLocation?.Invoke(this, body);
				}
			};
			await publishChannel.BasicConsumeAsync(locationConsumeQueue, autoAck: false, locationConsumer);
			Console.WriteLine($">>> Consumer registrovan na queue: {locationConsumeQueue}");



			var organizatorConsumer = new AsyncEventingBasicConsumer(publishChannel);

			organizatorConsumer.ReceivedAsync += async (_, ea) =>
			{
				string correlationId = ea.BasicProperties.CorrelationId;
                if (string.IsNullOrWhiteSpace(correlationId) || !_pendingRequest.TryRemove(correlationId, out var request))
                {
                    return;
                }
                string body = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine("PORUKA PRIMLJENA NA CONSUMEQUEUE: ORGANIZATOR " + body);
                #region commentedSmth
                //// dbFunkcija za proveru <postoji / ne postoji>
                //var props = new BasicProperties
                //{
                //    CorrelationId = correlationId,
                //    Persistent = true,
                //};
                //string result = "true";
                //byte[] byteBodyOrganizator = Encoding.UTF8.GetBytes(result);

                //await publishChannel.BasicPublishAsync(
                //		exchange: organizatorExchangeName,
                //		routingKey: ea.BasicProperties.ReplyTo,
                //		basicProperties: props,
                //		mandatory: false,
                //		body: byteBodyOrganizator
                //                );

                //await publishChannel.BasicAckAsync(ea.DeliveryTag, multiple:false);
                #endregion
                await publishChannel.BasicAckAsync(
					    ea.DeliveryTag,
					    multiple: false
					);
                // kada se obradi ovaj body -> ili ce biti true ili false (potvrdjuje ili ponistava akciju)
                OdgovorPrimljenOrganizator?.Invoke(this, body);
            };
            await publishChannel.BasicConsumeAsync(organizatorConsumeQueue, autoAck: false, organizatorConsumer);


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
