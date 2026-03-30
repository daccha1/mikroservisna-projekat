using mikroservisnaApp.Models.DTO.TipDogadjajaDTO;

namespace mikroservisnaApp.Contracts
{
	public interface ITipDogadjaja
	{
		public Task<List<TipDogadjajaResponseDTO>> GetAll();
		public Task<TipDogadjajaResponseDTO> GetById(int idTip);
		public Task<TipDogadjajaRequestDTO> Post(TipDogadjajaRequestDTO tip);
		public Task<bool> Update(int idTip, TipDogadjajaRequestDTO updatedTip);
	}
}
