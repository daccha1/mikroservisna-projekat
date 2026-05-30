using EventActivityService.Models.Events;

namespace EventActivityService.Models.Domain_models
{
	public abstract class AggregateRoot
	{
		public int ID { get; set; }
		protected readonly List<EventEntity> _unsavedEvents = [];
		public int Version { get; set; }

		protected void RaiseEvent(EventEntity @event)
		{
			Apply(@event);
			Version++;
			_unsavedEvents.Add(@event);
		}

		public abstract void Apply(EventEntity evt);

		public IReadOnlyList<EventEntity> DequeueUnsavedEvents()
		{
			var events = _unsavedEvents.ToList();
			_unsavedEvents.Clear();
			return events;
		}

		public void LoadEvents(List<EventEntity> events)
		{
			foreach(var evt in events)
			{
				Apply(evt);
				Version++;
			}
		}
	}
}
