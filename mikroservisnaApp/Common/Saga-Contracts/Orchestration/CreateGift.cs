using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Saga_Contracts
{
	public class CreateGift
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string Interesovanje { get; set; }
	}
}
