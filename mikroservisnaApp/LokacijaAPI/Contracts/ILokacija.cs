using mikroservisnaApp.Models.DTO.LokacijaDTO;

namespace ProductsAPI.Contracts
{
	public interface ILokacija
	{
		public Task<bool> Delete(int id);
		public Task<List<LokacijaResponseDTO>> GetAll();
		public Task<LokacijaResponseDTO> GetById(int idLocation);
		public Task<LokacijaRequestDTO> Post(LokacijaRequestDTO location);
		public Task<bool> Update(int idLocation, LokacijaRequestDTO updatedLocation);
	}
}
