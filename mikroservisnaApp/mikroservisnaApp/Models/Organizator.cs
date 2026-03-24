using System.ComponentModel.DataAnnotations.Schema;

namespace mikroservisnaApp.Models
{
    [Table("Organizator")]
    public class Organizator
    {
        public int Id { get; set; }
        public string Ime { get; set;  }
        public string Prezime { get; set; }

        // navs
        public List<StrucniDogadjaj> ListaDogadjaja { get; set; }

    }
}
