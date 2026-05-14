using Microsoft.EntityFrameworkCore;
using PosetilacAPI.Data;
using PosetilacAPI.Models;

namespace PosetilacAPI.Services
{
	public class OutboxBackgroundService : BackgroundService
	{
		private IServiceScopeFactory _scopeFactory;

		public OutboxBackgroundService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (true)
			{
				using var scope = _scopeFactory.CreateScope();
				var db = scope.ServiceProvider.GetService<PosetilacDbContext>();

				var msg = await db.SagaResultOutbox.Where(msg => msg.OutboxState == OutboxState.ForProcessing).FirstOrDefaultAsync();

				if (msg == null)
				{
					await Task.Delay(2000);
					continue;
				}
				;

				try
				{
					msg.OutboxState = OutboxState.Processed;

					var posetilacDbEntity = await db.Posetioci.Where(p => p.CorrelationId == msg.CorrelationId).FirstOrDefaultAsync();

					switch (msg.FinalState)
					{
						case State.Success: // ostaje sacuvano u bazi
							break;
						case State.Fail: // brise iz baze
							db.Remove(posetilacDbEntity);
							break;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Greska: Finalna realizacija transakcije neuspesna");
					msg.OutboxState = OutboxState.ForProcessing;
				}
				finally
				{
					db.Update(msg);
					await db.SaveChangesAsync();
				}
				;

				await Task.Delay(2000);
			}
		}

	}
}
