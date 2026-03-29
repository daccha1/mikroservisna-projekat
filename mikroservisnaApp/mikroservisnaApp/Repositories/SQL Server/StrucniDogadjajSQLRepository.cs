using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;

namespace mikroservisnaApp.Repositories.SQL_Server
{
	public class StrucniDogadjajSQLRepository : IStrucniDogadjaj
	{

		private DogadjajiDbContext context;

		public StrucniDogadjajSQLRepository(DogadjajiDbContext context)
		{
			this.context = context;
		}

		public async Task<StrucniDogadjaj> Add(StrucniDogadjaj dogadjaj)
		{
			throw new NotImplementedException();
		}

		public async Task<List<StrucniDogadjaj>> GetAll()
		{
			var events = await context.Dogadjaji.ToListAsync();

			if (events == null)
			{
				// error
			}

			return events;
		}

		public Task<StrucniDogadjaj> GetById(int idEvent)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Remove(int idEvent)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Update(int idEvent, StrucniDogadjaj updatedEvent)
		{
			throw new NotImplementedException();
		}
	}
}
