using System;
using System.Collections.Generic;
using System.Text;

namespace PosetilacSagaOrkestrator.Models
{
	public enum GiftOutboxStatus
	{
		ForProcessing,
		Processed
	}

	public class GiftOutboxMessage
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; }
		public GiftOutboxStatus Status { get; set; }

	}
}
