
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace EventsAPIGateway
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Configuration
						.SetBasePath(builder.Environment.ContentRootPath)
						.AddOcelot(); 
			
			builder.Services.AddOcelot(builder.Configuration);

			builder.Services.AddControllers();
			builder.Services.AddOpenApi();

			// Swagger
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.MapOpenApi();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();
			app.UseSwagger();
			app.UseSwaggerUI();
			await app.UseOcelot();
			app.Run();
		}
	}
}
