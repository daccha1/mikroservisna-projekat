using Common.StrucniDogadjajDTO;
using Microsoft.AspNetCore.Mvc;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.StrucniDogadjajDTO;

namespace mikroservisnaApp.Contracts
{
	public interface IStrucniDogadjaj
	{
		public Task<List<StrucniDogadajajResponseDTO>> GetAll();
		public Task<StrucniDogadjajDetailsResponseDTO> GetById(int idEvent);
		public Task<StrucniDogadjajRequestDTO> Post(StrucniDogadjajRequestDTO dogadjaj);
		//public Task<bool> Remove(int idEvent);
		public Task<bool> Update(int idEvent, StrucniDogadjajRequestDTO updatedEvent);

	}
}


//public Task<IActionResult> GetByLocation(int idLocation);
//public Task<IActionResult> GetByEventType(int idType);
//public Task<IActionResult> GetByOrganization(int idOrganizer);
// dodaj u todo: koji sve usecase-evi ce postojati za ovo da ne pises dzabe
