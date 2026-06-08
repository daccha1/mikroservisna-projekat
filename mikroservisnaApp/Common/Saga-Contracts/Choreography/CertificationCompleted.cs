using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Saga_Contracts.Choreography
{
	public enum CertificationState
	{
		Sucessful,
		Cancelled
	}

	public class CertificationCompleted
	{
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; }
		public CertificationState State { get; set; }
		public string? FailReason { get; set; } = null;

	}
}
