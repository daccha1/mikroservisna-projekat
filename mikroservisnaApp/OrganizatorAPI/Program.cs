using OrganizatorAPI.Contracts;
using OrganizatorAPI.Data;
using OrganizatorAPI.Repositories;

namespace OrganizatorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSqlServer<OrganizatorDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddScoped<IOrganizator, OrganizatorSQLRepository>();
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
