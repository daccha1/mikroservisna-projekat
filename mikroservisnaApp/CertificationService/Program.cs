using CertificationService.Services.RabbitMQ;

namespace CertificationService
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Certification service has been started.");
			bool kraj = false;

			MQClient client = new();
			await client.StartClient();
			
			
				
				
				
			Console.WriteLine("Upisi 1 za kraj, 0 za nastavak");
			var end = Console.ReadLine();
			if (end == "1") kraj = true;
			
		}
	}
}
