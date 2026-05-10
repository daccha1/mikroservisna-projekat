using Common.Saga_Contracts;
using GiftService.Models;

namespace GiftService.Contracts
{
	public interface IGift
	{
		public Task<Gift> CreateGift(Gift g);
		public List<GiftResponseDTO> GetGifts();
		public GiftResponseDTO GetGiftById(int id);
		Task CreateGiftOutboxMessage(GiftCreatedOutboxMessage outboxMsg);
	}
}
