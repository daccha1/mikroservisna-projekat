using Common;

namespace PosetilacAPI.Models
{
	public class CertificationRequestOutboxMessage
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public Status Status { get; set; } = Status.NotProcessed;
	}
}