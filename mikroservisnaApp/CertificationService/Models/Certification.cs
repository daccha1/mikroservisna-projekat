using System;
using System.Collections.Generic;
using System.Text;

namespace CertificationService.Models
{
	public enum CertificationStatus 
	{
		Certified,
		NotCertified,
		Cancelled
	}
	public class Certification
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; }
		public CertificationStatus CertificationStatus { get; set; } = CertificationStatus.NotCertified;
		public DateTime? CertifiedAt { get; set; } = null;

	}
}
