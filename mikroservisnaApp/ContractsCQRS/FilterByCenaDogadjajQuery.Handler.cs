using System;
using System.Collections.Generic;
using System.Text;

namespace ContractsCQRS
{
	public class FilterByCenaDogadjajQueryHandler
	{
		private IDogadjajReadRepository _repo;
		public FilterByCenaDogadjajQueryHandler(IDogadjajReadRepository repo)
		{
			_repo = repo;
		}

		public async Task<List<StrucniDogadjaj.Domain.Read.StrucniDogadjaj>> Handle(FilterByCenaDogadjajQuery query)
		{
			var result = await _repo.FilterByCenaDogadjaj(query.Cena);
			return result;
		}
	}
}
