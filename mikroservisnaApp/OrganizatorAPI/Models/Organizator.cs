using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizatorAPI.Models
{
    [Table("Organizator")]
    public class Organizator
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
