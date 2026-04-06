using Microsoft.EntityFrameworkCore;
using ProductsAPI.Models;

namespace LokacijaAPI.Data
{
	public class LokacijaDbContext : DbContext
	{
		public LokacijaDbContext(DbContextOptions options) : base(options) {}

		protected LokacijaDbContext() {}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Lokacija>().HasData(
				new Lokacija { Id = 1, Naziv = "Skladište Beograd", Adresa = "Bulevar Mihajla Pupina 10, Beograd", Kapacitet = 500 },
				new Lokacija { Id = 2, Naziv = "Skladište Novi Sad", Adresa = "Bulevar Oslobođenja 45, Novi Sad", Kapacitet = 300 },
				new Lokacija { Id = 3, Naziv = "Skladište Niš", Adresa = "Obrenovićeva 22, Niš", Kapacitet = 200 },
				new Lokacija { Id = 4, Naziv = "Skladište Kragujevac", Adresa = "Kralja Petra I 8, Kragujevac", Kapacitet = 150 },
				new Lokacija { Id = 5, Naziv = "Skladište Subotica", Adresa = "Korzo 3, Subotica", Kapacitet = 250 }
			);
		}

		public DbSet<Lokacija> Lokacije { get; set; }
	
	}
}
