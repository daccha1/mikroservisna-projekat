using Common.Saga_Contracts;
using GiftService.Contracts;
using GiftService.Data;
using GiftService.Models;
using Microsoft.EntityFrameworkCore;

namespace GiftService.Repositories.SQL_Server
{
	public class GiftSQLRepository : IGift
	{

		private GiftDbContext _context;

		public GiftSQLRepository(GiftDbContext context)
		{
			_context = context;
		}


		public async Task<Gift> CreateGift(Gift g)
		{
			_context.Gifts.Add(g);
			var isAdded = await _context.SaveChangesAsync();
			if(isAdded > 0)
			{
				return g;
			}
			return null;
		}

		public async Task CreateGiftOutboxMessage(GiftCreatedOutboxMessage outboxMsg)
		{
			_context.GiftCreatedOutboxTable.Add(outboxMsg);
			await _context.SaveChangesAsync();

		}

		public GiftResponseDTO GetGiftById(int id)
		{
			return new GiftResponseDTO();
		}

		public List<GiftResponseDTO> GetGifts()
		{
			var gifts = _context.Gifts.ToList();
			List<GiftResponseDTO> listaGiftova = new();
			
			foreach(var g in gifts)
			{
				GiftResponseDTO dtoGift = new()
				{
					Id = g.Id,
					CorrelationId = g.CorrelationId,
					Instrukcije = g.Instrukcije,
					Prirucnik = g.Prirucnik,
					Vaucer = g.Vaucer,
				};
				listaGiftova.Add(dtoGift);
			}
			
			return listaGiftova;
		}
	}
}
