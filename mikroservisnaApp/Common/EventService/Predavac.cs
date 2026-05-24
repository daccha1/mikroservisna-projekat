using System.ComponentModel.DataAnnotations.Schema;

namespace Common.EventService
{
    [Table("Predavac")]
    public class Predavac
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Titula { get; set; }
        public string OblastStrucnosti { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // Navigations
        public List<DogadjajPredavac> DogadjajPredavac { get; set; }



    }
}
