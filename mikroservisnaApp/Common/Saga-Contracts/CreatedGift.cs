using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Saga_Contracts
{
	public enum GiftStatus
	{
		Created,
		Failed
	}

	public class CreatedGift
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public GiftStatus GiftStatus { get; set; } = GiftStatus.Created;

	}
}
