namespace PosetilacAPI.MQ_Container
{
	public class MQInitializer : BackgroundService, IDisposable
	{
		private IMQClient _mqClient;
		public MQInitializer(IMQClient mqClient)
		{
			_mqClient = mqClient;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await _mqClient.EnsureStarted();
			Console.WriteLine("STARTED THE MQ CLIENT");
		}
	}
}
