using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CertificationService.Models
{
	public class CertificationCreatedOutboxMessage
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; }
		public Status Status { get; set; }
	}
}
