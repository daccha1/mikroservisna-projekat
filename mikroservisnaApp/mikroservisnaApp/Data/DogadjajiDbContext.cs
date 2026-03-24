using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Models;

namespace mikroservisnaApp.Data
{
    public class DogadjajiDbContext : DbContext
    {
        public DogadjajiDbContext(DbContextOptions options) : base(options)
        {
        }

        protected DogadjajiDbContext()
        {
        }

        public DbSet<Lokacija> Lokacije{ get; set; }
        public DbSet<Predavac> Predavaci { get; set; }
        public DbSet<StrucniDogadjaj> Dogadjaji { get; set; }
        public DbSet<Organizator> Organizatori { get; set; }
        public DbSet<TipDogadjaja> TipoviDogadjaja { get; set; }
        public DbSet<DogadjajPredavac> DogPreds{ get; set; }




    }
}
