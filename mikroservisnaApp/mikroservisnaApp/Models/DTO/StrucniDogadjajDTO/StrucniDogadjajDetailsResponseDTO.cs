namespace mikroservisnaApp.Models.DTO.StrucniDogadjajDTO
{
	public class StrucniDogadjajDetailsResponseDTO
	{
		public int Id { get; set; }
		public string Naziv { get; set; }
		public string Agenda { get; set; }
		public DateTime DatumVreme { get; set; }
		public int Trajanje { get; set; }
		public double Cena { get; set; }

		public string NazivLokacije { get; set; }
		public string NazivTipa { get; set; }
		public string NazivOrganizatora { get; set; }
		public List<string> Predavaci { get; set; }
	}
}
