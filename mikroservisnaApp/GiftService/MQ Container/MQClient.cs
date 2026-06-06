using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace GiftService.MQ_Container
{
	public interface IMQClient
	{
		public Task EnsureStarted();
		public Task SendMessage(string payload);
		event AsyncEventHandler<BasicDeliverEventArgs> HandleReceive;
		event AsyncEventHandler<BasicDeliverEventArgs> HandleCompensation;
	}

	public class MQClient : IMQClient
	{
		public ConnectionFactory factory;
		public IConnection connection;
		public IChannel channel;
		public event AsyncEventHandler<BasicDeliverEventArgs> HandleReceive;
		public event AsyncEventHandler<BasicDeliverEventArgs> HandleCompensation;

		public string exchangeName = "saga-exchange";

		public string pubGiftPosetilac = "events.posetilac.create-gift";
		public string pubGiftPosetilacRouting = "create-gift";

		public string giftCompensationQueue = "events.compensation.gift";
		public string giftCompensationRouting = "gift-compensation";

		// QUEUE NA KOJI CE ORKESTRATOR OSLUSKIVATI
		public string orchConsumeQueue = "events.orch.consume-queue";
		public string orchConsumeRouting = "gift-created";

		public async Task SendMessage(string payload)
		{
			byte[] byteBody = Encoding.UTF8.GetBytes(payload);

			await channel.BasicPublishAsync(
					exchange: exchangeName,
					routingKey: orchConsumeRouting,
					mandatory: true,
					body: byteBody
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
					queue: pubGiftPosetilac,
					durable: false,
					exclusive: false,
					autoDelete: false
				);

			await channel.QueueBindAsync(
					queue: pubGiftPosetilac,
					exchange: exchangeName,
					routingKey: pubGiftPosetilacRouting
				);

			// deklaracija queue-a na koji consumer slusa
			await channel.QueueDeclareAsync(
					queue: orchConsumeQueue,
					durable: false,
					exclusive: false,
					autoDelete: false
				);

			await channel.QueueBindAsync(
					queue: orchConsumeQueue,
					exchange: exchangeName,
					routingKey: orchConsumeRouting
				);
			// queue: gift kompenzacije
			await channel.QueueDeclareAsync(
					queue: giftCompensationQueue,
					durable: false,
					exclusive: false,
					autoDelete: false
				);
			await channel.QueueBindAsync(
					queue: giftCompensationQueue,
					exchange: exchangeName,
					routingKey: giftCompensationRouting
				);

			// consumer-a koji osluskuje queue
			AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);

			consumer.ReceivedAsync += async (_, ea) =>
			{
				await HandleReceive.Invoke(this, ea);
				await channel.BasicAckAsync(
						ea.DeliveryTag,
						multiple: false
					);
			};

			await channel.BasicConsumeAsync(
					queue: pubGiftPosetilac,
					autoAck: false,
					consumer: consumer
				);

			AsyncEventingBasicConsumer compensationConsumer = new AsyncEventingBasicConsumer(channel);

			compensationConsumer.ReceivedAsync += async (_, ea) =>
			{
				await HandleCompensation.Invoke(this, ea);
				await channel.BasicAckAsync(
						ea.DeliveryTag,
						multiple: false
					);
			};

			await channel.BasicConsumeAsync(
					queue: giftCompensationQueue,
					autoAck: false,
					consumer: compensationConsumer
				);

		}


	}
}
