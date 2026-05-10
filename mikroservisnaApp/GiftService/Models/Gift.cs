namespace GiftService.Models
{
	public enum GiftType
	{
		CyberSecurityPDF,
		MachineLearningPDF,
		CloudComputingPDF,
		DistributedSystemsPDF,
		WebDevelopmentPDF,
		EmbeddedProgrammingPDF
	}

	public class Gift
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; } // odnosi se na posetioca kom ce biti urucen poklon
		public GiftType Prirucnik { get; set; }
		public string Interesovanje { get; set; }
		public Guid Vaucer { get; set; } = Guid.NewGuid();
		public string Instrukcije { get; set; }

	}
}
