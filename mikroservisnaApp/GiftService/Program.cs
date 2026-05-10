
using GiftService.Contracts;
using GiftService.Data;
using GiftService.MQ_Container;
using GiftService.Repositories.SQL_Server;
using GiftService.Services;

namespace GiftService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddSingleton<IMQClient, MQClient>();
			builder.Services.AddHostedService<MQInitializer>();

			builder.Services.AddSqlServer<GiftDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

			builder.Services.AddHostedService<GiftOutboxTableReader>();

			builder.Services.AddScoped<IGiftEventsService, GiftEventsService>();
			builder.Services.AddScoped<IGift, GiftSQLRepository>();

			builder.Services.AddControllers();
			
			builder.Services.AddOpenApi();

			var app = builder.Build();

			
			if (app.Environment.IsDevelopment())
			{
				app.MapOpenApi();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
