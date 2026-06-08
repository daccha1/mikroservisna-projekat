using Common;
using Common.Saga_Contracts.Choreography;
using Microsoft.EntityFrameworkCore;
using PosetilacAPI.Data;
using PosetilacAPI.Models;
using PosetilacAPI.MQ_Container;

namespace PosetilacAPI.Services
{
	public class CertificationOutboxBackgroundService : BackgroundService
	{
		private IServiceScopeFactory _scopeFactory;
		private IMQClient _mqClient;
		public CertificationOutboxBackgroundService(IServiceScopeFactory scopeFactory, IMQClient mqClient)
		{
			_scopeFactory = scopeFactory;
			_mqClient = mqClient;
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (true)
			{
				using var scope = _scopeFactory.CreateScope();
				var db = scope.ServiceProvider.GetService<PosetilacDbContext>();

				var msg = await db.CertificationRequests.Where(msg => msg.Status == Status.NotProcessed).FirstOrDefaultAsync();

				if (msg == null)
				{
					await Task.Delay(2000);
					continue;
				};

				try
				{
					CertificationRequested request = new()
					{
						CorrelationId = msg.CorrelationId
					};

					await _mqClient.SendMessage(request);

					msg.Status = Status.Processed;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Nastala je greska pri izvrsavanju");
					msg.Status = Status.NotProcessed;
				}
				finally
				{
					db.Update(msg);
					await db.SaveChangesAsync();
				};

				await Task.Delay(2000);
			}
		}
	}
}
