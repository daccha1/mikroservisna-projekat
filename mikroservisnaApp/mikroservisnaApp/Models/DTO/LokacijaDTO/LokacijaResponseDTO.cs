using mikroservisnaApp.Models.DTO.StrucniDogadjajDTO;

namespace mikroservisnaApp.Models.DTO.LokacijaDTO
{
	public class LokacijaResponseDTO
	{
		public int Id { get; set; }
		public string Naziv { get; set; }
		public string Adresa { get; set; }
		public int Kapacitet { get; set; }
		public List<string> SpisakDogadjaja { get; set; }

	}
}
