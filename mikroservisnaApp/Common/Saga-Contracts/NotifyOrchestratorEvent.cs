using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Saga_Contracts
{
	public enum EventType
	{
		GiftEvent,
		NotificationEvent,
		Default
	}

	public enum GiftStatus
	{
		Created,
		Failed,
		None
	}

	public enum NotificationStatus
	{

	}

	public class NotifyOrchestratorEvent
	{
		public EventType EventType { get; set; } = EventType.Default;
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public GiftStatus GiftStatus { get; set; } = GiftStatus.None;


	}
}
