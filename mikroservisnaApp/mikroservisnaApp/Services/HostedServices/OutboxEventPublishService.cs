using Common;
using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Data;
using mikroservisnaApp.MQ_Container;
using System.Text.Json;

namespace mikroservisnaApp.Services.HostedServices
{
	public class OutboxEventPublishService : BackgroundService
	{
		private IServiceScopeFactory _scopeFactory;
		private IMQClient _mqClient;

		public OutboxEventPublishService(IServiceScopeFactory scopeFactory, IMQClient mqClient)
		{
			_mqClient = mqClient;
			_scopeFactory = scopeFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					Console.WriteLine(">>> BackgroundService provera >>>");

					using var scope = _scopeFactory.CreateScope();
					var db = scope.ServiceProvider.GetRequiredService<DogadjajiDbContext>();

					var outboxEntries = await db.OutboxTable.OrderBy(o => o.SentTime).Take(5).ToListAsync(stoppingToken);

					if (outboxEntries.Count == 0)
					{
						await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
						continue;
					}

					try
					{
						foreach (var entry in outboxEntries)
						{
							Console.WriteLine(">>> BGS pokupio podatke iz Outbox tabele");
							string jsonMessage = JsonSerializer.Serialize<OutboxMessage>(entry);
							await _mqClient.SendMessageAsync(jsonMessage, "events.event.eventsExchange", "event-publish-key", stoppingToken, messageId: entry.Id.ToString());
							db.OutboxTable.Remove(entry);
							await db.SaveChangesAsync(stoppingToken);
							Console.WriteLine(">>> BGS Zavrsena obrada podataka iz baze");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Publish error: " + ex.Message);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("EXCEPTION: OUTBOX PUBLISH SERVICE: " + ex.Message);
				}

				await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
			}

			
		}
	}
}
