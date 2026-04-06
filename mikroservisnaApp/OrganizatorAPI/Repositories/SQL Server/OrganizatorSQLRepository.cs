using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Models.DTO.OrganizatorDTO;
using OrganizatorAPI.Contracts;
using OrganizatorAPI.Data;

namespace OrganizatorAPI.Repositories
{
    public class OrganizatorSQLRepository : IOrganizator
    {
        private OrganizatorDbContext context;

        public OrganizatorSQLRepository(OrganizatorDbContext context)
        {
            this.context = context;
        }

        public async Task<List<OrganizatorResponseDTO>> GetAll()
        {
            var organizatori = await context.Organizatori.Select(o => new OrganizatorResponseDTO
            {
                Id = o.Id,
                Ime = o.Ime,
                Prezime = o.Prezime,
                ListaDogadjaja = new List<string>()
            }).ToListAsync();

            return organizatori;
        }

        public async Task<OrganizatorResponseDTO> GetById(int idOrganizator)
        {
            var organizator = await context.Organizatori
                .Where(o => o.Id == idOrganizator)
                .Select(o => new OrganizatorResponseDTO
                {
                    Id = o.Id,
                    Ime = o.Ime,
                    Prezime = o.Prezime,
                    ListaDogadjaja = new List<string>()
                }).FirstOrDefaultAsync();

            return organizator;
        }

        public async Task<OrganizatorRequestDTO> Post(OrganizatorRequestDTO organizator)
        {
            Models.Organizator noviOrganizator = new()
            {
                Ime = organizator.Ime,
                Prezime = organizator.Prezime,
            };

            context.Organizatori.Add(noviOrganizator);
            int isAdded = await context.SaveChangesAsync();
            if (isAdded != 0)
            {
                return organizator;
            }
            return null;
        }

        public async Task<bool> Update(int idOrganizator, OrganizatorRequestDTO updatedOrganizator)
        {
            var organizator = await context.Organizatori
                .Where(o => o.Id == idOrganizator)
                .FirstOrDefaultAsync();

            if (organizator == null)
            {
                return false;
            }

            organizator.Ime = updatedOrganizator.Ime;
            organizator.Prezime = updatedOrganizator.Prezime;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
