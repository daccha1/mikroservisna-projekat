using Microsoft.EntityFrameworkCore;
using OrganizatorAPI.Models;

namespace OrganizatorAPI.Data
{
    public class OrganizatorDbContext : DbContext
    {
        public OrganizatorDbContext(DbContextOptions options) : base(options) { }

        protected OrganizatorDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Organizator>().HasData(
                new Organizator { Id = 1, Ime = "Marko", Prezime = "Petrović", Email = "marko.petrovic@email.com", Password = "password123" },
                new Organizator { Id = 2, Ime = "Ana", Prezime = "Jovanović", Email = "ana.jovanovic@email.com", Password = "password123" },
                new Organizator { Id = 3, Ime = "Stefan", Prezime = "Nikolić", Email = "stefan.nikolic@email.com", Password = "password123" }
            );
        }

        public DbSet<Organizator> Organizatori { get; set; }
    }
}
