using EmailService.Data;
using EmailService.Models;
using EmailService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Resend;

namespace EmailService
{
	internal class Program
	{
		
		static async Task Main(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<EmailServiceDbContext>();
			optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EmailServiceDb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");

			using var context = new EmailServiceDbContext(optionsBuilder.Options);

			EmailSenderClient.Instance.StartClient();
			MQConsumer client = new();
			await client.StartClient();

			Console.WriteLine("Upisi nesto za kraj");
			Console.ReadLine();

		}
	}
}
