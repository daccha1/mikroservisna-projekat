using mikroservisnaApp.Models.DTO.PredavacDTO;

namespace mikroservisnaApp.Contracts
{
	public interface IPredavac
	{
		public Task<List<PredavacResponseDTO>> GetAll();
		public Task<PredavacResponseDTO> GetById(int idPredavac);
		public Task<PredavacRequestDTO> Post(PredavacRequestDTO predavac);
		public Task<bool> Update(int idPredavac, PredavacRequestDTO updatedPredavac);
	}
}
