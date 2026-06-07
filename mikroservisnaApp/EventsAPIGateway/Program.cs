
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using MMLib.SwaggerForOcelot.DependencyInjection;

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
			
			builder.Services.AddOcelot(builder.Configuration).AddCacheManager(x => x.WithDictionaryHandle());

			builder.Services.AddControllers();
			builder.Services.AddOpenApi();

			builder.Services.AddSwaggerForOcelot(builder.Configuration); // swagger for ocelot


			builder.Services.AddEndpointsApiExplorer();
			//builder.Services.AddSwaggerGen();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.MapOpenApi();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			app.UseSwaggerForOcelotUI(opt =>
			{
				opt.PathToSwaggerGenerator = "/swagger/docs";
			});

			await app.UseOcelot();
			app.Run();
		}
	}
}
