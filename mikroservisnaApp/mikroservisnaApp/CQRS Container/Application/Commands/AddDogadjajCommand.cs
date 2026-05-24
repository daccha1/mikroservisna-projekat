namespace mikroservisnaApp.CQRS_Container.Application.Commands
{
	public class AddDogadjajCommand
	{
		public string Naziv { get; set; }
		public string Agenda { get; set; }
		public DateTime DatumVreme { get; set; }
		public int Trajanje { get; set; }
		public double Cena { get; set; }
		public int LokacijaId { get; set; }
		public int TipId { get; set; }
		public int OrganizatorId { get; set; }
	}
}
