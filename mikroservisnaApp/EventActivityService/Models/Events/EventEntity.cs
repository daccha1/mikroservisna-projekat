using System.Diagnostics;

namespace EventActivityService.Models.Events
{
	public class EventEntity
	{
		public int Id { get; set; }
		public EventEntity()
		{
			EventId = Guid.NewGuid();
		}

		public Guid EventId { get; set; }
		public Guid UserCorrelationId { get; set; } 
		public DateTime OccuredAt { get; set; } = DateTime.UtcNow;

	}
}
