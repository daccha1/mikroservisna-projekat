using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Saga_Contracts
{
	public class PosetilacCreated
	{
		public Guid Id { get; set; }
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; }
		public string Interesovanje { get; set; }
	}
}
