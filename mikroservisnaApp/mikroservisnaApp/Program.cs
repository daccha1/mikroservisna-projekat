
using Microsoft.OpenApi;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Repositories.SQL_Server;
using System.Text.Json.Serialization;

namespace mikroservisnaApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSqlServer<DogadjajiDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
			builder.Services.AddScoped<IStrucniDogadjaj, StrucniDogadjajSQLRepository>();
			builder.Services.AddScoped<ILokacija, LokacijaSQLRepository>();

			builder.Services.AddControllers();

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Management", Version = "v1" });
			});

			var app = builder.Build();

            app.UseHttpsRedirection();
			
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				});
			}

			app.UseAuthorization();
			app.MapControllers();

            app.Run();
        }
    }
}
