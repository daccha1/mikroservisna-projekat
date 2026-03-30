using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.DogadjajPredavacDTO;

namespace mikroservisnaApp.Repositories.SQL_Server
{
	public class DogadjajPredavacSQLRepository : IDogadjajPredavac
	{
		private DogadjajiDbContext context;

		public DogadjajPredavacSQLRepository(DogadjajiDbContext context)
		{
			this.context = context;
		}

		public async Task<List<DogadjajPredavacResponseDTO>> GetAll()
		{
			var stavke = await context.DogPreds.Select(dp => new DogadjajPredavacResponseDTO
			{
				Id = dp.Id,
				RasporedPredavanja = dp.RasporedPredavanja,
				ImePrezimePredavaca = dp.Predavac.Ime + " " + dp.Predavac.Prezime,
				NazivDogadjaja = dp.StrucniDogadjaj.Naziv
			}).ToListAsync();
			
			// ne mora stavke == null jer se proverava u kontroleru svakako
			return stavke;
		}

		public async Task<DogadjajPredavacResponseDTO> GetById(int idDogadjajPredavac)
		{
			var stavka = await context.DogPreds
				.Where(dp => dp.Id == idDogadjajPredavac)
				.Select(dp => new DogadjajPredavacResponseDTO
				{
					Id = dp.Id,
					RasporedPredavanja = dp.RasporedPredavanja,
					ImePrezimePredavaca = dp.Predavac.Ime + " " + dp.Predavac.Prezime,
					NazivDogadjaja = dp.StrucniDogadjaj.Naziv
				}).FirstOrDefaultAsync();
			
			return stavka;
		}

		public async Task<DogadjajPredavacRequestDTO> Post(DogadjajPredavacRequestDTO dogadjajPredavac)
		{
			var predavacPostoji = await context.Predavaci.AnyAsync(p => p.Id == dogadjajPredavac.PredavacId);
			var dogadjajPostoji = await context.Dogadjaji.AnyAsync(d => d.Id == dogadjajPredavac.StrucniDogadjajId);

			if (!predavacPostoji || !dogadjajPostoji)
			{
				return null;
			}

			DogadjajPredavac novi = new()
			{
				RasporedPredavanja = dogadjajPredavac.RasporedPredavanja,
				PredavacId = dogadjajPredavac.PredavacId,
				StrucniDogadjajId = dogadjajPredavac.StrucniDogadjajId
			};

			context.DogPreds.Add(novi);
			int isAdded = context.SaveChanges();
			if (isAdded != 0)
			{
				return dogadjajPredavac;
			}
			return null;
		}

		public async Task<bool> Update(int idDogadjajPredavac, DogadjajPredavacRequestDTO updatedDogadjajPredavac)
		{
			var stavka = await context.DogPreds
				.Where(dp => dp.Id == idDogadjajPredavac)
				.FirstOrDefaultAsync();

			if (stavka == null)
			{
				return false;
			}

			var predavacPostoji = await context.Predavaci.AnyAsync(p => p.Id == updatedDogadjajPredavac.PredavacId);
			var dogadjajPostoji = await context.Dogadjaji.AnyAsync(d => d.Id == updatedDogadjajPredavac.StrucniDogadjajId);

			if (!predavacPostoji || !dogadjajPostoji)
			{
				return false;
			}

			stavka.RasporedPredavanja = updatedDogadjajPredavac.RasporedPredavanja;
			stavka.PredavacId = updatedDogadjajPredavac.PredavacId;
			stavka.StrucniDogadjajId = updatedDogadjajPredavac.StrucniDogadjajId;

			context.SaveChanges();
			return true;
		}
	}
}
