using mikroservisnaApp.Models.DTO.OrganizatorDTO;

namespace OrganizatorAPI.Contracts
{
    public interface IOrganizator
    {
        public Task<List<OrganizatorResponseDTO>> GetAll();
        public Task<OrganizatorResponseDTO> GetById(int idOrganizator);
        public Task<OrganizatorRequestDTO> Post(OrganizatorRequestDTO organizator);
        public Task<bool> Update(int idOrganizator, OrganizatorRequestDTO updatedOrganizator);
    }
}
