using Microsoft.AspNetCore.Connections;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using OrganizatorAPI.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OrganizatorAPI.MQ_Container
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

        // organizator
        string organizatorExchangeName = "events.organizer.organizerExchange";
        string organizatorQueueName = "events.organizer.organizerQueue";
        string organizatorRoutingKey = "organizer-publish-key";

        string organizatorConsumeQueue = "events.organizer.consumeQueue";
        string organizatorConsumeKey = "organizer-consume-key";


        public MQClient(IServiceScopeFactory scope)
        {
            this.scopeFactory = scope;
        }

        public async Task<bool> ProveriOrganizatora(int id)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrganizatorDbContext>();

            var organizator = dbContext.Organizatori.Where(o => o.Id == id).FirstOrDefault();

            if (organizator == null)
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
                int idOrg = Convert.ToInt32(payload);
                string odgovor = "false";
                bool doesExist = await ProveriOrganizatora(idOrg);

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
                Console.WriteLine($">>> Odgovor: '{odgovor}'");

                await publishChannel.BasicPublishAsync(
                        organizatorExchangeName,
                        routingKey: ea.BasicProperties.ReplyTo,
                        body: bodyResponse,
                        basicProperties: props,
                        mandatory: false
                );
                Console.WriteLine(">>> basicpublish pozvan");

                await publishChannel.BasicAckAsync(ea.DeliveryTag, false);

            };
            await publishChannel.BasicConsumeAsync(
                    organizatorQueueName,
                    false,
                    consumer
                );
           


            //await ReceiveMessageAsync();

        }


    }
}
