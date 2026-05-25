using ContractsCQRS;

namespace mikroservisnaApp.CQRS_Container.Application.Queries
{
	public class GetDetailsDogadjajQueryHandler
	{
		private IDogadjajReadRepository _repo;
		public GetDetailsDogadjajQueryHandler(IDogadjajReadRepository repo)
		{
			_repo = repo;
		}

		public Task<StrucniDogadjaj.Domain.Read.StrucniDogadjaj> Handle(GetDetailsDogadjajQuery query)
		{
			var result = _repo.GetDetailsDogadjaj(query.IdDogadjaja);
			return result;
		}
	}
}
