using OrganizatorAPI.Contracts;
using OrganizatorAPI.Data;
using OrganizatorAPI.MQ_Container;
using OrganizatorAPI.Repositories;

namespace OrganizatorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSqlServer<OrganizatorDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddSingleton<IMQClient, MQClient>();

            builder.Services.AddHostedService<MQInitializer>();

            builder.Services.AddScoped<IOrganizator, OrganizatorSQLRepository>();
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
