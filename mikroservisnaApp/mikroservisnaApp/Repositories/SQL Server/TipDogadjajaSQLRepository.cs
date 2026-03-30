using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.TipDogadjajaDTO;

namespace mikroservisnaApp.Repositories.SQL_Server
{
	public class TipDogadjajaSQLRepository : ITipDogadjaja
	{
		private DogadjajiDbContext context;

		public TipDogadjajaSQLRepository(DogadjajiDbContext context)
		{
			this.context = context;
		}

		public async Task<List<TipDogadjajaResponseDTO>> GetAll()
		{
			var tipovi = await context.TipoviDogadjaja.Select(t => new TipDogadjajaResponseDTO
			{
				Id = t.Id,
				Naziv = t.Naziv,
				ListaDogadjaja = t.ListaDogadjaja.Select(d => d.Naziv).ToList()
			}).ToListAsync();

			return tipovi;
		}

		public async Task<TipDogadjajaResponseDTO> GetById(int idTip)
		{
			var tip = await context.TipoviDogadjaja
				.Where(t => t.Id == idTip)
				.Select(t => new TipDogadjajaResponseDTO
				{
					Id = t.Id,
					Naziv = t.Naziv,
					ListaDogadjaja = t.ListaDogadjaja.Select(d => d.Naziv).ToList()
				}).FirstOrDefaultAsync();

			return tip;
		}

		public async Task<TipDogadjajaRequestDTO> Post(TipDogadjajaRequestDTO tip)
		{
			TipDogadjaja noviTip = new()
			{
				Naziv = tip.Naziv
			};

			context.TipoviDogadjaja.Add(noviTip);
			int isAdded = context.SaveChanges();
			if (isAdded != 0)
			{
				return tip;
			}
			return null;
		}

		public async Task<bool> Update(int idTip, TipDogadjajaRequestDTO updatedTip)
		{
			var tip = await context.TipoviDogadjaja
				.Where(t => t.Id == idTip)
				.FirstOrDefaultAsync();

			if (tip == null)
			{
				return false;
			}

			tip.Naziv = updatedTip.Naziv;

			context.SaveChanges();
			return true;
		}
	}
}
