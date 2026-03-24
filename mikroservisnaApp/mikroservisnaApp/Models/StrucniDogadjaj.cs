using Microsoft.AspNetCore.Components.Routing;
using System.ComponentModel.DataAnnotations.Schema;

namespace mikroservisnaApp.Models
{
    [Table("Dogadjaj")]
    public class StrucniDogadjaj
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Agenda { get; set; }
        public DateTime DatumVreme { get; set; }
        public int Trajanje { get; set; } // trajanje u minutima (ex. 120min, 60min, 90min...)
        public double Cena { get; set; }


        // navigation IDs
        public int LokacijaId { get; set; }
        public int TipId { get; set; }
        public int OrganizatorId { get; set; }
        
        // navigation

        public TipDogadjaja Tip { get; set; }
        public List<DogadjajPredavac> SpisakPredavaca { get; set; } 
        public Lokacija Lokacija { get; set; }
        public Organizator Organizator { get; set; }

    }
}