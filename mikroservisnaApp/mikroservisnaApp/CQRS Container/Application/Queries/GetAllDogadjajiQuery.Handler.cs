using ContractsCQRS;
using Microsoft.AspNetCore.Mvc;

namespace mikroservisnaApp.CQRS_Container.Application.Queries
{
	public class GetAllDogadjajiQueryHandler
	{
		private IDogadjajReadRepository _repo;
		public GetAllDogadjajiQueryHandler(IDogadjajReadRepository repo)
		{
			_repo = repo;
		}

		public async Task<List<StrucniDogadjaj.Domain.Read.StrucniDogadjaj>> Handle(GetAllDogadjajiQuery query)
		{
			var events = await _repo.GetAllDogadjaji();
			if (events != null) return events;
			return null;
		}
		
		
	}
}
