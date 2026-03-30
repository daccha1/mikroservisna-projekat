namespace mikroservisnaApp.Models.DTO.TipDogadjajaDTO
{
	public class TipDogadjajaResponseDTO
	{
		public int Id { get; set; }
		public string Naziv { get; set; }
		public List<string> ListaDogadjaja { get; set; }
	}
}
