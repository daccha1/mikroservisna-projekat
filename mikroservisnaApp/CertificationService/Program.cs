using CertificationService.Services;
using CertificationService.Services.RabbitMQ;

namespace CertificationService
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Certification service has been started.");
			
			MQClient client = new();
			await client.StartClient();

			_ = Task.Run(() => DispatcherService.DispatchCreatedOutboxMessage());


			Console.WriteLine("Upisi 1 za kraj, 0 za nastavak");
			var end = Console.ReadLine();
			
		}
	}
}
