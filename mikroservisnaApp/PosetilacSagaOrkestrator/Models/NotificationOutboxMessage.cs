using System;
using System.Collections.Generic;
using System.Text;

namespace PosetilacSagaOrkestrator.Models
{

	public enum NotificationOutboxStatus
	{
		ForProcessing,
		Processed
	}

	public class NotificationOutboxMessage
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public NotificationOutboxStatus Status { get; set; } = NotificationOutboxStatus.ForProcessing;

	}
}
