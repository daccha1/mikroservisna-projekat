using System.ComponentModel.DataAnnotations.Schema;

namespace Common.EventService
{
    [Table("Dogadjaj_Predavac")]
    public class DogadjajPredavac
    {
        public int Id { get; set; }
        public DateTime RasporedPredavanja { get; set; }
        public int PredavacId { get; set; }
        public int StrucniDogadjajId { get; set; }


        // navigation
        public Predavac Predavac { get; set; }
        public StrucniDogadjaj StrucniDogadjaj { get; set; }

    }
}
