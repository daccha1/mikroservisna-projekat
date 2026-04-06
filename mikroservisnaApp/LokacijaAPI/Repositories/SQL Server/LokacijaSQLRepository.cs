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

		public Task<bool> Update(int idLocation, LokacijaRequestDTO updatedLocation)
		{
			return Task.FromResult(false);
		}
	}
}
