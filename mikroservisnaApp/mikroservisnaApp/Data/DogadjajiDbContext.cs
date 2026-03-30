using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Models;

namespace mikroservisnaApp.Data
{
    public class DogadjajiDbContext : DbContext
    {
        public DogadjajiDbContext(DbContextOptions options) : base(options) {}

        protected DogadjajiDbContext() {}

        public DbSet<Lokacija> Lokacije{ get; set; }
        public DbSet<Predavac> Predavaci { get; set; }
        public DbSet<StrucniDogadjaj> Dogadjaji { get; set; }
        public DbSet<Organizator> Organizatori { get; set; }
        public DbSet<TipDogadjaja> TipoviDogadjaja { get; set; }
        public DbSet<DogadjajPredavac> DogPreds{ get; set; }


		#region ModelCreating
		// SeedData za bazu
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// TipDogadjaja
			modelBuilder.Entity<TipDogadjaja>().HasData(
				new TipDogadjaja { Id = 1, Naziv = "Konferencija" },
				new TipDogadjaja { Id = 2, Naziv = "Radionica" },
				new TipDogadjaja { Id = 3, Naziv = "Seminar" },
				new TipDogadjaja { Id = 4, Naziv = "Webinar" }
			);

			// Lokacija
			modelBuilder.Entity<Lokacija>().HasData(
				new Lokacija { Id = 1, Naziv = "Tehnološki park Beograd", Adresa = "Veljka Dugoševića 54, Beograd", Kapacitet = 300 },
				new Lokacija { Id = 2, Naziv = "Novi Sad IT Hub", Adresa = "Bulevar Oslobođenja 12, Novi Sad", Kapacitet = 150 },
				new Lokacija { Id = 3, Naziv = "Smart City Centar", Adresa = "Nemanjina 4, Beograd", Kapacitet = 80 }
			);

			// Organizator
			modelBuilder.Entity<Organizator>().HasData(
				new Organizator { Id = 1, Ime = "Marko", Prezime = "Nikolić" },
				new Organizator { Id = 2, Ime = "Ana", Prezime = "Jovanović" },
				new Organizator { Id = 3, Ime = "Stefan", Prezime = "Petrović" }
			);

			// Predavac
			modelBuilder.Entity<Predavac>().HasData(
				new Predavac { Id = 1, Ime = "Nikola", Prezime = "Đorđević", Email = "nikola.djordjevic@gmail.com", Password = "hashed_pass_1", Titula = "Dr.", OblastStrucnosti = "Veštačka inteligencija" },
				new Predavac { Id = 2, Ime = "Jelena", Prezime = "Stojanović", Email = "jelena.stojanovic@gmail.com", Password = "hashed_pass_2", Titula = "Prof.", OblastStrucnosti = "Web razvoj" },
				new Predavac { Id = 3, Ime = "Milan", Prezime = "Vasić", Email = "milan.vasic@gmail.com", Password = "hashed_pass_3", Titula = "Mr.", OblastStrucnosti = "Baze podataka" },
				new Predavac { Id = 4, Ime = "Ivana", Prezime = "Lukić", Email = "ivana.lukic@gmail.com", Password = "hashed_pass_4", Titula = "Dr.", OblastStrucnosti = "Kibernetička bezbednost" }
			);

			// StrucniDogadjaj
			modelBuilder.Entity<StrucniDogadjaj>().HasData(
				new StrucniDogadjaj
				{
					Id = 1,
					Naziv = "AI Summit Srbija 2025",
					Agenda = "Panel diskusije o primeni AI u industriji, radionice, networking",
					DatumVreme = new DateTime(2025, 9, 15, 10, 0, 0),
					Trajanje = 480,
					Cena = 4999.99,
					LokacijaId = 1,
					OrganizatorId = 1,
					TipId = 1
				},
				new StrucniDogadjaj
				{
					Id = 2,
					Naziv = "Web Dev Radionica",
					Agenda = "React, Next.js, moderne prakse u web razvoju",
					DatumVreme = new DateTime(2025, 10, 5, 9, 0, 0),
					Trajanje = 240,
					Cena = 2499.99,
					LokacijaId = 2,
					OrganizatorId = 2,
					TipId = 2
				},
				new StrucniDogadjaj
				{
					Id = 3,
					Naziv = "Baze Podataka — napredne tehnike",
					Agenda = "Query optimizacija, indeksiranje, NoSQL vs SQL",
					DatumVreme = new DateTime(2025, 11, 20, 11, 0, 0),
					Trajanje = 180,
					Cena = 1999.99,
					LokacijaId = 3,
					OrganizatorId = 3,
					TipId = 3
				}
			);

			// DogadjajPredavac
			modelBuilder.Entity<DogadjajPredavac>().HasData(
				new DogadjajPredavac { Id = 1, StrucniDogadjajId = 1, PredavacId = 1, RasporedPredavanja = new DateTime(2025, 9, 15, 10, 0, 0) },
				new DogadjajPredavac { Id = 2, StrucniDogadjajId = 1, PredavacId = 4, RasporedPredavanja = new DateTime(2025, 9, 15, 13, 0, 0) },
				new DogadjajPredavac { Id = 3, StrucniDogadjajId = 2, PredavacId = 2, RasporedPredavanja = new DateTime(2025, 10, 5, 9, 0, 0) },
				new DogadjajPredavac { Id = 4, StrucniDogadjajId = 3, PredavacId = 3, RasporedPredavanja = new DateTime(2025, 11, 20, 11, 0, 0) }
			);


		}
		#endregion

	}
}
