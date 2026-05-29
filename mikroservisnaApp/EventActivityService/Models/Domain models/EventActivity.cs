using EventActivityService.Models.Events;

namespace EventActivityService.Models.Domain_models
{
	public class EventActivity : AggregateRoot
	{
		public Guid GuestId { get; set; }
		public EventHall CurrentHall { get; set; }
		public decimal Balance { get; set; }
		public string ContactedCompany { get; set; }
		public DateTime CheckedInAt { get; set; }
		public DateTime CheckedOutAt { get; set; }


		public static EventActivity GuestCheckedIn(Guid guestId)
		{
			var activity = new EventActivity();

			var @evt = new GuestCheckedIn()
			{
				GuestId = guestId
			};

			activity.RaiseEvent(@evt);

			return activity;
		}

		public override void Apply(EventEntity evt)
		{
			switch (evt)
			{
				case GuestCheckedIn ev:
					GuestId = ev.GuestId;
					CurrentHall = EventHall.CheckIn;
					Balance = 0m;
					ContactedCompany = "";
					CheckedInAt = DateTime.UtcNow;
					CheckedOutAt = DateTime.MinValue;
					break;
			}
		}
	}
}
