using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace mikroservisnaApp.Models
{
    [Table("Lokacija")]
    public class Lokacija
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Adresa { get; set; }
        public int Kapacitet { get; set; }


        // navigation
        public List<StrucniDogadjaj> ListaDogadjaja { get; set; }

    }
}