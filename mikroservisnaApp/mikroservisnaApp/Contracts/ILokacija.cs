using mikroservisnaApp.Models.DTO.LokacijaDTO;
using mikroservisnaApp.Models.DTO.StrucniDogadjajDTO;

namespace mikroservisnaApp.Contracts
{
	public interface ILokacija
	{
		public Task<List<LokacijaResponseDTO>> GetAll();
		public Task<LokacijaResponseDTO> GetById(int idLocation);
		public Task<LokacijaRequestDTO> Post(LokacijaRequestDTO location);
		public Task<bool> Update(int idLocation, LokacijaRequestDTO updatedLocation);
	}
}
