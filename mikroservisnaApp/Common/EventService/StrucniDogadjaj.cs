using System.ComponentModel.DataAnnotations.Schema;

namespace Common.EventService
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
        public TipDogadjaja Tip { get; set; }
        public List<DogadjajPredavac> SpisakPredavaca { get; set; } 
        public Lokacija Lokacija { get; set; }
        public Organizator Organizator { get; set; }

    }
}