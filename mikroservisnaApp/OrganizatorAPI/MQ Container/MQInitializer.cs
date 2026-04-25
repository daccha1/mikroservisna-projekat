namespace OrganizatorAPI.MQ_Container
{
    public class MQInitializer : IHostedService
    {
        IMQClient _mqClient;
        public MQInitializer(IMQClient client)
        {
            _mqClient = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _mqClient.EnsureStarted();
            Console.WriteLine("POKRENUT MQ CLIENT");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
