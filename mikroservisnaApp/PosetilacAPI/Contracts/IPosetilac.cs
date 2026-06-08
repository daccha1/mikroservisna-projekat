using mikroservisnaApp.Models.DTO.PosetilacDTO;

namespace PosetilacAPI.Contracts
{
    public interface IPosetilac
    {
        public Task<List<PosetilacResponseDTO>> GetAll();
        public Task<PosetilacResponseDTO> GetById(int idPosetilac);
		public Task<bool> IssueCertification(int id);
		public Task<PosetilacRequestDTO> Post(PosetilacRequestDTO posetilac);
        public Task<bool> Update(int idPosetilac, PosetilacRequestDTO updatedPosetilac);
    }
}
