using System;
using System.Collections.Generic;
using System.Text;
using StrucniDogadjaj.Domain.Read;

namespace ContractsCQRS
{
	public interface IDogadjajReadRepository
	{
		Task<List<StrucniDogadjaj.Domain.Read.StrucniDogadjaj>> GetAllDogadjaji();
		Task<StrucniDogadjaj.Domain.Read.StrucniDogadjaj> GetDetailsDogadjaj(int id);
		Task<List<StrucniDogadjaj.Domain.Read.StrucniDogadjaj>> FilterByCenaDogadjaj(double cena);
	}
}
