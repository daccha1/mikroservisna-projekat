using System.ComponentModel.DataAnnotations.Schema;

namespace PosetilacAPI.Models
{
    [Table("Posetilac")]
    public class Posetilac
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string StatusZaposlenja { get; set; }
        public string Interesovanje { get; set; }
        public int DogadjajId { get; set; }

    }
}
