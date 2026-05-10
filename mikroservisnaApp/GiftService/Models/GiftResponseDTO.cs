namespace GiftService.Models
{
	public class GiftResponseDTO
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; } // odnosi se na posetioca kom ce biti urucen poklon
		public GiftType Prirucnik { get; set; }
		public Guid Vaucer { get; set; }
		public string Instrukcije { get; set; }
	}
}
