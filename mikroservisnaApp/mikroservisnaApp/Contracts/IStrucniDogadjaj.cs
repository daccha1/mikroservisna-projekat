using Microsoft.AspNetCore.Mvc;
using mikroservisnaApp.Models;

namespace mikroservisnaApp.Contracts
{
	public interface IStrucniDogadjaj
	{
		public Task<List<StrucniDogadjaj>> GetAll();
		public Task<StrucniDogadjaj> GetById(int idEvent);
		public Task<StrucniDogadjaj> Add(StrucniDogadjaj dogadjaj);
		public Task<bool> Remove(int idEvent);
		public Task<bool> Update(int idEvent, StrucniDogadjaj updatedEvent);

	}
}


//public Task<IActionResult> GetByLocation(int idLocation);
//public Task<IActionResult> GetByEventType(int idType);
//public Task<IActionResult> GetByOrganization(int idOrganizer);
// dodaj u todo: koji sve usecase-evi ce postojati za ovo da ne pises dzabe
