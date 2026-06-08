using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Saga_Contracts.Choreography
{
	public class CertificationEmailFailed
	{
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
