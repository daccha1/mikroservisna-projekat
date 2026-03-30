namespace mikroservisnaApp.Models.DTO.PredavacDTO
{
	public class PredavacResponseDTO
	{
		public int Id { get; set; }
		public string Ime { get; set; }
		public string Prezime { get; set; }
		public string Titula { get; set; }
		public string OblastStrucnosti { get; set; }
		public string Email { get; set; }
		public List<string> ListaDogadjaja { get; set; }
	}
}
