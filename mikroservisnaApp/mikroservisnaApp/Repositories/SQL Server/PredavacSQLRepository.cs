using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.PredavacDTO;

namespace mikroservisnaApp.Repositories.SQL_Server
{
	public class PredavacSQLRepository : IPredavac
	{
		private DogadjajiDbContext context;

		public PredavacSQLRepository(DogadjajiDbContext context)
		{
			this.context = context;
		}

		public async Task<List<PredavacResponseDTO>> GetAll()
		{
			var predavaci = await context.Predavaci.Select(p => new PredavacResponseDTO
			{
				Id = p.Id,
				Ime = p.Ime,
				Prezime = p.Prezime,
				Titula = p.Titula,
				OblastStrucnosti = p.OblastStrucnosti,
				Email = p.Email
			}).ToListAsync();

			return predavaci;
		}

		public async Task<PredavacResponseDTO> GetById(int idPredavac)
		{
			var predavac = await context.Predavaci
				.Where(p => p.Id == idPredavac)
				.Select(p => new PredavacResponseDTO
				{
					Id = p.Id,
					Ime = p.Ime,
					Prezime = p.Prezime,
					Titula = p.Titula,
					OblastStrucnosti = p.OblastStrucnosti,
					Email = p.Email
				}).FirstOrDefaultAsync();

			return predavac;
		}

		public async Task<PredavacRequestDTO> Post(PredavacRequestDTO predavac)
		{
			Predavac noviPredavac = new()
			{
				Ime = predavac.Ime,
				Prezime = predavac.Prezime,
				Titula = predavac.Titula,
				OblastStrucnosti = predavac.OblastStrucnosti,
				Email = predavac.Email,
				Password = predavac.Password
			};

			context.Predavaci.Add(noviPredavac);
			int isAdded = context.SaveChanges();
			if (isAdded != 0)
			{
				return predavac;
			}
			return null;
		}

		public async Task<bool> Update(int idPredavac, PredavacRequestDTO updatedPredavac)
		{
			var predavac = await context.Predavaci
				.Where(p => p.Id == idPredavac)
				.FirstOrDefaultAsync();

			if (predavac == null)
			{
				return false;
			}

			predavac.Ime = updatedPredavac.Ime;
			predavac.Prezime = updatedPredavac.Prezime;
			predavac.Titula = updatedPredavac.Titula;
			predavac.OblastStrucnosti = updatedPredavac.OblastStrucnosti;
			predavac.Email = updatedPredavac.Email;
			predavac.Password = updatedPredavac.Password;

			context.SaveChanges();
			return true;
		}
	}
}
