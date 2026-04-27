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
					using var scope = _scopeFactory.CreateScope();
					var db = scope.ServiceProvider.GetService<DogadjajiDbContext>();

					var outboxEntries = await db.OutboxTable.OrderBy(o => o.SentTime).Take(5).ToListAsync(stoppingToken);

					if (outboxEntries.Count == 0)
					{
						break;
					}

					try
					{
						foreach (var entry in outboxEntries)
						{
							string jsonMessage = JsonSerializer.Serialize<OutboxMessage>(entry);
							await _mqClient.SendMessageAsync(jsonMessage, "events.event.eventsExchange", "event-publish-key", stoppingToken);
							db.OutboxTable.Remove(entry);
							await db.SaveChangesAsync(stoppingToken);
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

				await Task.Delay(TimeSpan.FromSeconds(3));
			}

			
		}
	}
}
