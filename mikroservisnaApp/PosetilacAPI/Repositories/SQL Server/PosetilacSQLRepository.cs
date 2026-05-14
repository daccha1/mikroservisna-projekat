using Common.Saga_Contracts;
using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Models.DTO.PosetilacDTO;
using PosetilacAPI.Contracts;
using PosetilacAPI.Data;
using PosetilacAPI.Models;
using PosetilacAPI.MQ_Container;

namespace PosetilacAPI.Repositories
{
    public class PosetilacSQLRepository : IPosetilac
    {
        private PosetilacDbContext context;
        private IMQClient _mqClient;
        public PosetilacSQLRepository(PosetilacDbContext context, IMQClient mqClient)
        {
            this.context = context;
            _mqClient = mqClient;

		}

        public async Task<List<PosetilacResponseDTO>> GetAll()
        {
            var posetioci = await context.Posetioci.Select(p => new PosetilacResponseDTO
            {
                Id = p.Id,
                Ime = p.Ime,
                Prezime = p.Prezime,
                StatusZaposlenja = p.StatusZaposlenja,
                Interesovanje = p.Interesovanje,
                DogadjajId = p.DogadjajId,
                ListaDogadjaja = new List<string>()
            }).ToListAsync();

            return posetioci;
        }

        public async Task<PosetilacResponseDTO> GetById(int idPosetilac)
        {
            var posetilac = await context.Posetioci
                .Where(p => p.Id == idPosetilac)
                .Select(p => new PosetilacResponseDTO
                {
                    Id = p.Id,
                    Ime = p.Ime,
                    Prezime = p.Prezime,
                    StatusZaposlenja = p.StatusZaposlenja,
                    Interesovanje = p.Interesovanje,
                    DogadjajId = p.DogadjajId,
                    ListaDogadjaja = new List<string>()
                }).FirstOrDefaultAsync();

            return posetilac;
        }

        public async Task<PosetilacRequestDTO> Post(PosetilacRequestDTO posetilac)
        {
            Posetilac noviPosetilac = new()
            {
                Ime = posetilac.Ime,
                Prezime = posetilac.Prezime,
                StatusZaposlenja = posetilac.StatusZaposlenja,
                Interesovanje = posetilac.Interesovanje,
                DogadjajId = posetilac.DogadjajId,
                CorrelationId = Guid.NewGuid()
            };

            PosetilacCreated posetilacCreatedEvent = new()
            {
				Id = Guid.NewGuid(),
				CorrelationId = noviPosetilac.CorrelationId,
                CreatedAt = DateTime.UtcNow,
                Interesovanje = noviPosetilac.Interesovanje
            };

            await _mqClient.SendMessage(posetilacCreatedEvent);

			// publishujemo poruku --> ...saga...
			//                     <-- response
			// ^ pokrenemo save u contextu za posetioca        

			context.Posetioci.Add(noviPosetilac);
			int isAdded = await context.SaveChangesAsync();

			if (isAdded != 0)
			{
				return posetilac;
			}
            return null;
			
		}

        public async Task<bool> Update(int idPosetilac, PosetilacRequestDTO updatedPosetilac)
        {
            var posetilac = await context.Posetioci
                .Where(p => p.Id == idPosetilac)
                .FirstOrDefaultAsync();

            if (posetilac == null)
            {
                return false;
            }

            posetilac.Ime = updatedPosetilac.Ime;
            posetilac.Prezime = updatedPosetilac.Prezime;
            posetilac.StatusZaposlenja = updatedPosetilac.StatusZaposlenja;
            posetilac.Interesovanje = updatedPosetilac.Interesovanje;
            posetilac.DogadjajId = updatedPosetilac.DogadjajId;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
