using EmailService.Services;
using Resend;
using EmailService.Models;

namespace EmailService
{
	internal class Program
	{
		
		static async Task Main(string[] args)
		{
			EmailSenderClient.Instance.StartClient();
			MQConsumer client = new();
			await client.StartClient();

			Console.WriteLine("Upisi nesto za kraj");
			Console.ReadLine();

		}
	}
}
