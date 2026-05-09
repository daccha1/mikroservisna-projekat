using Microsoft.EntityFrameworkCore;
using PosetilacAPI.Models;

namespace PosetilacAPI.Data
{
    public class PosetilacDbContext : DbContext
    {
        public PosetilacDbContext(DbContextOptions options) : base(options) { }

        protected PosetilacDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Posetilac>().HasData(
                new Posetilac { Id = 1, Ime = "Petar", Prezime = "Ilić", StatusZaposlenja = "Zaposlen", Interesovanje = "Vestacka Inteligencija", DogadjajId = 1 },
                new Posetilac { Id = 2, Ime = "Milica", Prezime = "Stanković", StatusZaposlenja = "Student", Interesovanje = "Web development", DogadjajId = 2 },
                new Posetilac { Id = 3, Ime = "Jovan", Prezime = "Marković", StatusZaposlenja = "Nezaposlen", Interesovanje = "Machine Learning", DogadjajId = 3 },
                new Posetilac { Id = 4, Ime = "Ana", Prezime = "Pavlović", StatusZaposlenja = "Stalni radni odnos", Interesovanje = "Embedded", DogadjajId = 1 },
                new Posetilac { Id = 5, Ime = "Nikola", Prezime = "Savić", StatusZaposlenja = "Student", Interesovanje = "Mikroservisi", DogadjajId = 2 }
            );
        }

        public DbSet<Posetilac> Posetioci { get; set; }
    }
}
