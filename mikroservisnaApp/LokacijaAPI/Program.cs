
using LokacijaAPI.Data;
using LokacijaAPI.Services;
using ProductsAPI.Contracts;
using ProductsAPI.Repositories;

namespace ProductsAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddSqlServer<LokacijaDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

			builder.Services.AddSingleton<IMQClient, MQClient>();
			builder.Services.AddHostedService<MQInitializer>();
			builder.Services.AddScoped<ILokacija, LokacijaSQLRepository>();
			builder.Services.AddControllers();

			builder.Services.AddOpenApi();

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
				{
					policy
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader();
				});
			});

			var app = builder.Build();
			
			// Configure the HTTP request pipeline.
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
