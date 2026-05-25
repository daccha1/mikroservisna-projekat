
using Microsoft.OpenApi;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.MQ_Container;
using mikroservisnaApp.Patterns;
using mikroservisnaApp.Repositories.SQL_Server;
using mikroservisnaApp.Services.HostedServices;
using System.Text.Json.Serialization;
using StrucniDogadjaj.Infrastructure.Write.EFCore.Data;
using StrucniDogadjaj.Infrastructure.Read.EFCore.Data;
using ContractsCQRS;
using mikroservisnaApp.CQRS_Container.Application.Repositories;
using mikroservisnaApp.CQRS_Container.Application.Commands;
using mikroservisnaApp.CQRS_Container.Application.Queries;

namespace mikroservisnaApp
{
    public class Program
    {
		private WriteDbContext _dbContext;
		public Program(WriteDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSqlServer<DogadjajiDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
			builder.Services.AddSqlServer<ReadDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
			builder.Services.AddSqlServer<WriteDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

			builder.Services.AddTransient<IDogadjajWriteRepository, DogadjajWriteRepository>();
			builder.Services.AddTransient<IDogadjajReadRepository, DogadjajReadRepository>();

			builder.Services.AddScoped<AddDogadjajCommandHandler>();
			builder.Services.AddScoped<EditDogadjajCommandHandler>();
			builder.Services.AddScoped<DeleteDogadjajCommandHandler>();
			builder.Services.AddScoped<GetAllDogadjajiQueryHandler>();
			builder.Services.AddScoped<FilterByCenaDogadjajQueryHandler>();
			builder.Services.AddScoped<GetAllDogadjajiQueryHandler>();
			builder.Services.AddScoped<GetDetailsDogadjajQueryHandler>();

			builder.Services.Configure<HostOptions>(opt =>
			{
				opt.StartupTimeout = TimeSpan.FromSeconds(10);
				opt.ServicesStartConcurrently = true; // proveri
			});
			builder.Services.AddHostedService<OutboxEventPublishService>();

			builder.Services.AddSingleton<IMQClient, MQClient>();
			//builder.Services.AddHostedService<MQInitializer>();

			builder.Services.AddSingleton<CircuitBreaker>(sp =>
				new CircuitBreaker(1, TimeSpan.FromSeconds(7))
			);

			builder.Services.AddHttpClient("OrganizatorAPI", (client) =>
			{
				client.Timeout = TimeSpan.FromSeconds(5); // ne dobije se odgovor kroz 5 sekundi -> baca ex
				client.BaseAddress = new Uri(builder.Configuration["OrganizatorAPI"]);
			});

			builder.Services.AddHttpClient("LokacijaAPI", (client) =>
			{
				client.Timeout = TimeSpan.FromSeconds(2); // ne dobije se odgovor kroz 5 sekundi -> baca ex
				client.BaseAddress = new Uri(builder.Configuration["LokacijaAPI"]);
			});


			builder.Services.AddScoped<IStrucniDogadjaj, StrucniDogadjajSQLRepository>();
			builder.Services.AddScoped<ILokacija, LokacijaSQLRepository>();
			builder.Services.AddScoped<IDogadjajPredavac, DogadjajPredavacSQLRepository>();
			builder.Services.AddScoped<IOrganizator, OrganizatorSQLRepository>();
			builder.Services.AddScoped<IPredavac, PredavacSQLRepository>();
			builder.Services.AddScoped<ITipDogadjaja, TipDogadjajaSQLRepository>();

			builder.Services.AddControllers();

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Management", Version = "v1" });
			});

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

			app.UseCors("AllowAll");
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
