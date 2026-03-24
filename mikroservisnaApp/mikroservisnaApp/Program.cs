
using mikroservisnaApp.Data;

namespace mikroservisnaApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSqlServer<DogadjajiDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

            var app = builder.Build();


            app.UseHttpsRedirection();

           
            app.MapControllers();

            app.Run();
        }
    }
}
