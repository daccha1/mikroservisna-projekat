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
                new Posetilac { Id = 1, Ime = "Petar", Prezime = "Ilić", StatusZaposlenja = "Zaposlen", Interesovanje = "Vestacka Inteligencija", DogadjajId = 1, CorrelationId= new Guid("c5db59d7-9e3d-48fd-89c5-e433b5eaf01a") },
                new Posetilac { Id = 2, Ime = "Milica", Prezime = "Stanković", StatusZaposlenja = "Student", Interesovanje = "Web development", DogadjajId = 2, CorrelationId = new Guid("98230c8d-f5db-4202-9734-3385e7d11c1f") },
                new Posetilac { Id = 3, Ime = "Jovan", Prezime = "Marković", StatusZaposlenja = "Nezaposlen", Interesovanje = "Machine Learning", DogadjajId = 3, CorrelationId = new Guid("945a2aea-0eca-4322-936c-0608b7142372") },
                new Posetilac { Id = 4, Ime = "Ana", Prezime = "Pavlović", StatusZaposlenja = "Stalni radni odnos", Interesovanje = "Embedded", DogadjajId = 1, CorrelationId = new Guid("9fa651d7-3dce-4c73-aa03-b0eca675b7de")   },
                new Posetilac { Id = 5, Ime = "Nikola", Prezime = "Savić", StatusZaposlenja = "Student", Interesovanje = "Mikroservisi", DogadjajId = 2, CorrelationId = new Guid("29dfd66c-644f-409e-8b2b-1dfc4a0cb386") }
            );
        }

        public DbSet<Posetilac> Posetioci { get; set; }
    }
}
