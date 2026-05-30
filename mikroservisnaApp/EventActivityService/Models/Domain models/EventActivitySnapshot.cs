namespace EventActivityService.Models.Domain_models
{
	public class EventActivitySnapshot : AggregateSnapshot
	{
		public Guid GuestId { get; set; }
		public EventHall CurrentHall { get; set; }
		public decimal Balance { get; set; }
		public string ContactedCompany { get; set; }
		public DateTime CheckedInAt { get; set; }
		public DateTime? CheckedOutAt { get; set; }
	}
}
