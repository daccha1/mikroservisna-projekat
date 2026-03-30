using mikroservisnaApp.Models.DTO.DogadjajPredavacDTO;
using mikroservisnaApp.Models.DTO.DogadjajPredavacDTO;

namespace mikroservisnaApp.Contracts
{
	public interface IDogadjajPredavac
	{
		public Task<List<DogadjajPredavacResponseDTO>> GetAll();
		public Task<DogadjajPredavacResponseDTO> GetById(int idDogadjajPredavac);
		public Task<DogadjajPredavacRequestDTO> Post(DogadjajPredavacRequestDTO dogadjajPredavac);
		public Task<bool> Update(int idDogadjajPredavac, DogadjajPredavacRequestDTO updatedDogadjajPredavac);
	}
}
