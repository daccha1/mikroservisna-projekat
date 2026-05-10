namespace GiftService.Models
{
	public enum GiftOutboxStatus
	{
		ForProcessing,
		Processed
	}

	public class GiftCreatedOutboxMessage
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; }
		public GiftOutboxStatus Status { get; set; } = GiftOutboxStatus.ForProcessing;
	}
}
