using ContractsCQRS;
using Microsoft.EntityFrameworkCore;
using StrucniDogadjaj.Infrastructure.Read.EFCore.Data;

namespace mikroservisnaApp.CQRS_Container.Application.Repositories
{
	public class DogadjajReadRepository : IDogadjajReadRepository
	{
		private ReadDbContext _read;
		public DogadjajReadRepository(ReadDbContext read)
		{
			_read = read;
		}

		public async Task<List<StrucniDogadjaj.Domain.Read.StrucniDogadjaj>> FilterByCenaDogadjaj(double cena)
		{
			var events = await _read.Dogadjaji.Where(d => d.Cena <= cena).ToListAsync();
			return events;
		}

		public async Task<List<StrucniDogadjaj.Domain.Read.StrucniDogadjaj>> GetAllDogadjaji()
		{
			var events = await _read.Dogadjaji.ToListAsync();
			return events;
		}

		public async Task<StrucniDogadjaj.Domain.Read.StrucniDogadjaj> GetDetailsDogadjaj(int id)
		{
			var evt = await _read.Dogadjaji.Where(d => d.Id == id).FirstOrDefaultAsync();
			return evt;
		}
	}
}
