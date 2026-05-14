using EmailService.Data;
using EmailService.Models;
using EmailService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Resend;


namespace EmailService
{
	internal class Program
	{
		
		static async Task Main(string[] args)
		{
			var host = Host.CreateDefaultBuilder(args)
						   .ConfigureServices(services =>		
							{
								services.AddHostedService<MQConsumer>();
								services.AddDbContext<EmailServiceDbContext>(options => // premesti u dbContext da resis ove gluposti
								{
									options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EmailServiceDb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");
								});

							})
						   .Build();

			await host.RunAsync();


			Console.WriteLine("Upisi nesto za kraj");
			Console.ReadLine();

		}
	}
}
