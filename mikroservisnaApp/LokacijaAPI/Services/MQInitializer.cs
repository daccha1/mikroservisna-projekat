namespace LokacijaAPI.Services
{
	public class MQInitializer : IHostedService
	{
		private readonly IMQClient _mqClient;

		public MQInitializer(IMQClient mqClient)
		{
			_mqClient = mqClient;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _mqClient.EnsureStarted();
			Console.WriteLine("POKRENUTO!!!");
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
