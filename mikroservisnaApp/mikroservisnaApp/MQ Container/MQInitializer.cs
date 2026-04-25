namespace mikroservisnaApp.MQ_Container
{
	public class MQInitializer : IHostedService
	{
		private readonly IMQClient _mqClient;
		public MQInitializer(IMQClient mqClient) => _mqClient = mqClient;

		public async Task StartAsync(CancellationToken ct) => await _mqClient.EnsureStarted();
		public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
	}
}
