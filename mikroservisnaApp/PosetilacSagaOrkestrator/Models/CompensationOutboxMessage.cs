using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PosetilacSagaOrkestrator.Models
{
	public enum Service
	{
		Gift
	}
	public class CompensationOutboxMessage
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; }
		public Service CompensationService { get; set; }
		public Status Status { get; set; }
	}
}
