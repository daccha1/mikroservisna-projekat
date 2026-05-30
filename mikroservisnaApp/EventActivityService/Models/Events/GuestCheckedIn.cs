namespace EventActivityService.Models.Events
{
	public class GuestCheckedIn
	{
		public Guid GuestId { get; set; }
		public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
	}
}
