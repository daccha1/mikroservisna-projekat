using EventActivityService.Data;
using EventActivityService.Models.Domain_models;
using EventActivityService.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace EventActivityService.Repositories.SQL_Server
{
	public class EventActivitySQLRepository
	{
		EventsDbContext context;
		public EventActivitySQLRepository(EventsDbContext ctx)
		{
			context = ctx;
		}

		public async Task<T> Load<T>(Guid guestId) where T : AggregateRoot, new()
		{
			var exists = await context.Events.Where(evt => evt.UserCorrelationId == guestId).FirstAsync();
			if (exists == null) return null;

			var snapshot = await context.ActivitySnapshots.Where(snapshot => snapshot.GuestId == guestId).OrderBy(snapshot=>snapshot.ID).LastOrDefaultAsync();
			T aggregate = new();
			int startFromVersion = 0;

			if (snapshot != null)
			{
				startFromVersion = snapshot.Version;
				var eventsAfterVersion = await context.Events.Where(evt => evt.UserCorrelationId == guestId).Skip(startFromVersion).ToListAsync();

				aggregate.ApplySnapshot(snapshot);
				aggregate.LoadEvents(eventsAfterVersion);
				return aggregate;
			}

			var events = await context.Events.Where(evt => evt.UserCorrelationId == guestId).ToListAsync();

			aggregate.LoadEvents(events);

			return aggregate;
		}

		public async Task<EventActivity> CheckIn(Guid guestId)
		{
			var doesExist = await context.Events.Where(evt => evt.UserCorrelationId == guestId).FirstOrDefaultAsync();
			if (doesExist != null) return null;

			var activity = EventActivity.GuestCheckedIn(guestId);
			await context.Events.AddRangeAsync(activity.DequeueUnsavedEvents());
			await context.SaveChangesAsync();
			return activity;
		}

		internal async Task<EventActivity> CheckOut(Guid guestId)
		{
			var doesExist = await context.Events.Where(evt => evt.UserCorrelationId == guestId).FirstOrDefaultAsync();
			if (doesExist == null) return null;

			var aggregate = await Load<EventActivity>(guestId);

			if (aggregate.CheckedOutAt == DateTime.MinValue || aggregate.CheckedOutAt < aggregate.CheckedInAt) return null;

			EventActivity activity = await EventActivity.CheckOut(aggregate, guestId);
			await context.Events.AddRangeAsync(activity.DequeueUnsavedEvents());
			await context.SaveChangesAsync();
			return activity;
		}

		internal async Task<EventActivity> AddBalance(Guid guestId, decimal balance)
		{
			var doesExist = await context.Events.Where(evt => evt.UserCorrelationId == guestId).FirstOrDefaultAsync();
			if (doesExist == null) return null;

			if (balance < 50) return null;

			var aggregate = await Load<EventActivity>(guestId);
			EventActivity activity = await EventActivity.AddBalance(aggregate, balance);
			await context.Events.AddRangeAsync(activity.DequeueUnsavedEvents());
			await context.SaveChangesAsync();
			return activity;
		}

		internal async Task<EventActivity> RemoveBalance(Guid guestId, decimal balance)
		{
			var doesExist = await context.Events.Where(evt => evt.UserCorrelationId == guestId).FirstOrDefaultAsync();
			if (doesExist == null) return null;

			var aggregate = await Load<EventActivity>(guestId);

			if (aggregate.Balance < 50) return null;

			EventActivity activity = await EventActivity.RemoveBalance(aggregate, balance);
			await context.Events.AddRangeAsync(activity.DequeueUnsavedEvents());
			await context.SaveChangesAsync();
			return activity;
		}

		internal async Task<EventActivity> SwitchHall(Guid guestId, int hallNumber)
		{
			var doesExist = await context.Events.Where(evt => evt.UserCorrelationId == guestId).FirstOrDefaultAsync();
			if (doesExist == null) return null;

			if (hallNumber > 3) return null;

			var aggregate = await Load<EventActivity>(guestId);
			EventActivity activity = await EventActivity.SwitchHall(aggregate, hallNumber);
			await context.Events.AddRangeAsync(activity.DequeueUnsavedEvents());
			await context.SaveChangesAsync();
			return activity;
		}

		// Prikaz svih eventova za nekog guesta
		internal async Task<List<EventEntity>> GetAllEvents(Guid guestId)
		{
			var evts = await context.Events.Where(evts => evts.UserCorrelationId == guestId).ToListAsync();
			return evts;
		}

		internal async Task CreateSnapshot(Guid guestId)
		{
			var aggr = await Load<EventActivity>(guestId);
			EventActivitySnapshot snapshot = new()
			{
				Balance = aggr.Balance,
				CheckedInAt = aggr.CheckedInAt,
				CheckedOutAt = aggr.CheckedOutAt,
				ContactedCompany = aggr.ContactedCompany,
				CurrentHall = aggr.CurrentHall,
				GuestId = aggr.GuestId,
				ID = aggr.ID,
				Version = aggr.Version
			};
			await context.ActivitySnapshots.AddAsync(snapshot);
			await context.SaveChangesAsync();
		}
	}
}
