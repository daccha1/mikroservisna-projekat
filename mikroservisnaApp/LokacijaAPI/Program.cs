
using LokacijaAPI.Data;
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



			builder.Services.AddScoped<ILokacija, LokacijaSQLRepository>();
			builder.Services.AddControllers();

			builder.Services.AddOpenApi();

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
