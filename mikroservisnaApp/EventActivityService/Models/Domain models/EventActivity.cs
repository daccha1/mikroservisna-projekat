using EventActivityService.Models.Events;
using EventActivityService.Repositories.SQL_Server;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace EventActivityService.Models.Domain_models
{
	public class EventActivity : AggregateRoot
	{
		public Guid GuestId { get; set; }
		public EventHall CurrentHall { get; set; }
		public decimal Balance { get; set; }
		public string ContactedCompany { get; set; }
		public DateTime CheckedInAt { get; set; }
		public DateTime? CheckedOutAt { get; set; }

	
		public static EventActivity GuestCheckedIn(Guid guestId)
		{
			var aggregate = new EventActivity();

			var evt = new GuestCheckedIn()
			{
				GuestId = guestId,
				CheckInTime = DateTime.UtcNow
			};

			var baseEvt = new EventEntity()
			{
				EventType = evt.GetType().Name,
				Payload = EventEntity.Serialize<GuestCheckedIn>(evt),
				UserCorrelationId = evt.GuestId
			};

			aggregate.RaiseEvent(baseEvt);

			return aggregate;
		}

		public static async Task<EventActivity> SwitchHall(EventActivity aggregate, int hallNumber)
		{
			var evt = new SwitchHallEvent()
			{
				HallNumber = hallNumber
			};

			var baseEvt = new EventEntity()
			{
				EventType = evt.GetType().Name,
				Payload = EventEntity.Serialize<SwitchHallEvent>(evt),
				UserCorrelationId = aggregate.GuestId
			};

			aggregate.RaiseEvent(baseEvt);

			return aggregate;
		}

		internal static async Task<EventActivity> AddBalance(EventActivity aggregate, decimal balance)
		{
			var evt = new AddBalanceEvent()
			{
				Amount = balance
			};

			var baseEvt = new EventEntity()
			{
				EventType = evt.GetType().Name,
				Payload = EventEntity.Serialize<AddBalanceEvent>(evt),
				UserCorrelationId = aggregate.GuestId
			};

			aggregate.RaiseEvent(baseEvt);
			return aggregate;
		}

		internal static async Task<EventActivity> CheckOut(EventActivity aggregate, Guid guestId)
		{
			var evt = new GuestCheckedOut()
			{
				CheckedOutAt = DateTime.UtcNow
			};

			var baseEvt = new EventEntity()
			{
				EventType = evt.GetType().Name,
				Payload = EventEntity.Serialize<GuestCheckedOut>(evt),
				UserCorrelationId = aggregate.GuestId
			};

			aggregate.RaiseEvent(baseEvt);

			return aggregate;
		}

		internal static async Task<EventActivity> RemoveBalance(EventActivity aggregate, decimal balance)
		{
			var evt = new RemoveBalanceEvent()
			{
				Amount = balance
			};

			var baseEvt = new EventEntity()
			{
				EventType = evt.GetType().Name,
				Payload = EventEntity.Serialize<RemoveBalanceEvent>(evt),
				UserCorrelationId = aggregate.GuestId
			};

			aggregate.RaiseEvent(baseEvt);

			return aggregate;

		}

		public override void Apply(EventEntity baseEvt)
		{
			switch (baseEvt.EventType)
			{
				case "GuestCheckedIn":
					var checkIn = EventEntity.Deserialize<GuestCheckedIn>(baseEvt.Payload);
					GuestId = checkIn.GuestId;
					CheckedInAt = checkIn.CheckInTime;
					CurrentHall = EventHall.CheckIn;
					Balance = 0m;
					ContactedCompany = "";
					CheckedOutAt = DateTime.MinValue;
					break;
				case "SwitchHallEvent":
					var switchHall = EventEntity.Deserialize<SwitchHallEvent>(baseEvt.Payload);
					CurrentHall = (EventHall)switchHall.HallNumber;
					break;
				case "AddBalanceEvent":
					var addBalance = EventEntity.Deserialize<AddBalanceEvent>(baseEvt.Payload);
					Balance += addBalance.Amount;
					break;
				case "RemoveBalanceEvent":
					var removeBalance = EventEntity.Deserialize<RemoveBalanceEvent>(baseEvt.Payload);
					Balance = Balance + removeBalance.Amount;
					break;
				case "GuestCheckedOut":
					var guestCheckedOut = EventEntity.Deserialize<GuestCheckedOut>(baseEvt.Payload);
					CheckedOutAt = guestCheckedOut.CheckedOutAt;
					break;
			}
		}

		public override void ApplySnapshot(AggregateSnapshot snapshot)
		{
			if (snapshot is not EventActivitySnapshot eventSnapshot)
				throw new Exception("Nije odgovarajuci event");

			GuestId = eventSnapshot.GuestId;
			CurrentHall = eventSnapshot.CurrentHall;
			Balance = eventSnapshot.Balance;
			ContactedCompany = eventSnapshot.ContactedCompany;
			CheckedInAt = eventSnapshot.CheckedInAt;
			CheckedOutAt = eventSnapshot.CheckedOutAt;
			Version = eventSnapshot.Version;
		}
	}
}
