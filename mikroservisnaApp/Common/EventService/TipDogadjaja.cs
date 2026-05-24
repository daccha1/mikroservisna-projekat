using System.ComponentModel.DataAnnotations.Schema;

namespace Common.EventService
{
    [Table("TipDogadjaja")]
    public class TipDogadjaja
    {
        public int Id { get; set; }
        public string Naziv { get; set; }

        // navigation
        public List<StrucniDogadjaj> ListaDogadjaja { get; set; }
    }
}