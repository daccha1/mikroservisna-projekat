using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Common.EventService;

namespace StrucniDogadjaj.Domain.Read
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
	}
}
