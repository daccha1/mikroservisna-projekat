using EventActivityService.Data;
using EventActivityService.Models.Domain_models;

namespace EventActivityService.Repositories.SQL_Server
{
	public class EventActivitySQLRepository
	{
		EventsDbContext context;
		public EventActivitySQLRepository(EventsDbContext ctx)
		{
			context = ctx;
		}

		public async Task CheckIn(Guid guestId)
		{
			var activity = EventActivity.GuestCheckedIn(guestId);

			await context.Events.AddRangeAsync(activity.DequeueUnsavedEvents());

		}
	}
}
