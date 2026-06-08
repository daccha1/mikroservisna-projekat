using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Saga_Contracts.Choreography
{
	public enum FailType
	{
		EmailFail,
		CertificationServiceFail
	}
	public class CertificationFailed
	{
		public Guid CorrelationId { get; set; }
		public FailType FailType { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
