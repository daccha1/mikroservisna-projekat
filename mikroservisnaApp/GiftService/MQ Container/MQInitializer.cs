using Common.Saga_Contracts;
using Microsoft.AspNetCore.Connections;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using RabbitMQ.Client;
using GiftService.Services;

namespace GiftService.MQ_Container
{
	public class MQInitializer : BackgroundService, IDisposable
	{
		private IMQClient _mqClient;
		private IServiceScopeFactory scopeFactory;
		public MQInitializer(IMQClient mqClient, IServiceScopeFactory scopeFc)
		{
			_mqClient = mqClient;
			scopeFactory = scopeFc;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await _mqClient.EnsureStarted();
			Console.WriteLine("STARTED THE MQ CLIENT");

			_mqClient.HandleReceive += async (_, ea) =>
			{
				using (var scope = scopeFactory.CreateScope()) 
				{
					var _giftService = scope.ServiceProvider.GetService<IGiftEventsService>();

					var jsonString = Encoding.UTF8.GetString(ea.Body.Span);
					PosetilacCreated? posetilac = JsonSerializer.Deserialize<PosetilacCreated>(jsonString);

					Console.WriteLine("Gift service je pokrenut i obradice poruku.");
					await _giftService.HandleGiftCreation(posetilac);
					Console.WriteLine("Gift service je obradio poruku. Gift je dodat u bazu.");
				}
			};

			_mqClient.HandleCompensation += async (_, ea) =>
			{
				using (var scope = scopeFactory.CreateScope())
				{
					var _giftService = scope.ServiceProvider.GetService<IGiftEventsService>();
					GiftCompensation? comp = JsonSerializer.Deserialize<GiftCompensation>(Encoding.UTF8.GetString(ea.Body.Span));

					Console.WriteLine($"Izvrsavanje kompenzacije - brisanje gift-a: {comp.CorrelationId}");
					await _giftService.ExecuteCompensation(comp.CorrelationId);				}
					Console.WriteLine("Gift izbrisan");
			};
			await Task.Delay(Timeout.Infinite, stoppingToken);
		}

	}
}
