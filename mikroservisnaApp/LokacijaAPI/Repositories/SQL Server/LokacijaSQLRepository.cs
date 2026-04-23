using LokacijaAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Models.DTO.LokacijaDTO;
using ProductsAPI.Contracts;


namespace ProductsAPI.Repositories
{
	public class LokacijaSQLRepository : ILokacija
	{
		private LokacijaDbContext context;
		public LokacijaSQLRepository(LokacijaDbContext context)
		{
			this.context = context;
		}

		public async Task<bool> Delete(int id)
		{
			bool a = true;
			return a;
		}

		public async Task<List<LokacijaResponseDTO>> GetAll()
		{
			var locations = await context.Lokacije.Select(l => new LokacijaResponseDTO
			{
				Id = l.Id,
				Naziv = l.Naziv,
				Adresa = l.Adresa,
				Kapacitet = l.Kapacitet
			}).ToListAsync();

			if (locations == null)
			{
				return null;
			}

			return locations;
		}

		public async Task<LokacijaResponseDTO> GetById(int idLocation)
		{
			var lokacija = await context.Lokacije.Where(l => l.Id == idLocation).FirstOrDefaultAsync();
		
			if(lokacija == null)
			{
				return null;
			}

			return new LokacijaResponseDTO
			{
				Id = lokacija.Id,
				Adresa = lokacija.Adresa,
				Kapacitet = lokacija.Kapacitet,
				Naziv = lokacija.Naziv
			};
		}

		public Task<LokacijaRequestDTO> Post(LokacijaRequestDTO location)
		{
			return Task.FromResult<LokacijaRequestDTO>(null);
		}

		public async Task<bool> Update(int idLocation, LokacijaRequestDTO updatedLocation)
		{
			var lokacija = await context.Lokacije.Where(l => l.Id == idLocation).FirstOrDefaultAsync();
			if(lokacija == null)
			{
				return false;
			}
			lokacija.Kapacitet = updatedLocation.Kapacitet;
			lokacija.Adresa = updatedLocation.Adresa;
			lokacija.Naziv = updatedLocation.Naziv;
			
			await context.SaveChangesAsync();
			return true;

		}
	}
}
