using Common.Saga_Contracts;
using GiftService.Data;
using GiftService.Models;
using GiftService.MQ_Container;
using System.Text.Json;

namespace GiftService.Services
{
	public class GiftOutboxTableReader : IHostedService
	{
		private IServiceScopeFactory _scopeFactory;
		private Task _executeOutboxRefresh;
		public GiftOutboxTableReader(IServiceScopeFactory scopeFc)
		{
			_scopeFactory = scopeFc;
		}

		public async Task ExecuteOutboxCheck(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				using (var scope = _scopeFactory.CreateScope()) 
				{
					var db = scope.ServiceProvider.GetService<GiftDbContext>();
					Console.WriteLine("CITAM");
					var outboxResult = db.GiftCreatedOutboxTable.Where(msg => msg.Status == GiftOutboxStatus.ForProcessing).FirstOrDefault();

					// ovaj msg treba se konvertuje u GiftCreated, i da se sibne na orkestrator opet

					if (outboxResult == null)
					{
						await Task.Delay(5000);
						continue;
					}

					CreatedGift giftMsg = new()
					{
						CorrelationId = outboxResult.CorrelationId
					};

					// ovo sad treba da se publishuje na orkestrator
					var mqClient = scope.ServiceProvider.GetService<IMQClient>();

					await mqClient.SendMessage(JsonSerializer.Serialize<CreatedGift>(giftMsg));

					outboxResult.Status = GiftOutboxStatus.Processed;
					db.GiftCreatedOutboxTable.Update(outboxResult);
					await db.SaveChangesAsync();
				}
				

				await Task.Delay(5000);
			}

		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_executeOutboxRefresh = Task.Run(() => ExecuteOutboxCheck(cancellationToken));

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("GASENJE GiftService!");
			return Task.CompletedTask;
		}
	}
}
