using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Common.StrucniDogadjajDTO
{

    public class PostPredavacRequestDTO
	{
		public DateTime RasporedPredavanja { get; set; }
		public int PredavacId { get; set; }

	}

	public class StrucniDogadjajRequestDTO
	{
		public string Naziv { get; set; }
		public string Agenda { get; set; }
		public DateTime DatumVreme { get; set; }
		public int Trajanje { get; set; }
		public double Cena { get; set; }

		public int LokacijaId { get; set; }
		public int TipId { get; set; }
		public int OrganizatorId { get; set; }
		

		public List<PostPredavacRequestDTO> Predavaci { get; set; }

		public override string ToString()
		{
			return $"Naziv: {Naziv}, Trajanje: {Trajanje}, Cena: {Cena}";
		}


	}
}
