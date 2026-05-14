using PosetilacAPI.Contracts;
using PosetilacAPI.Data;
using PosetilacAPI.MQ_Container;
using PosetilacAPI.Repositories;
using PosetilacAPI.Services;

namespace PosetilacAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSqlServer<PosetilacDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddSingleton<IMQClient, MQClient>();
            builder.Services.AddHostedService<MQInitializer>();
            builder.Services.AddHostedService<OutboxBackgroundService>();

            builder.Services.AddScoped<IPosetilac, PosetilacSQLRepository>();
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
