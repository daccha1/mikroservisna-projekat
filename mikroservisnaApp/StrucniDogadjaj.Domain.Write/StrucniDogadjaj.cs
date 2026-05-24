using Common.EventService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StrucniDogadjaj.Domain.Write
{
	[Table("Dogadjaj")]
	public class StrucniDogadjaj
	{
		public int Id { get; set; }
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
