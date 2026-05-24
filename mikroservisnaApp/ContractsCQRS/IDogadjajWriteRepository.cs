using System;
using System.Collections.Generic;
using System.Text;
using StrucniDogadjaj.Domain.Write;


namespace ContractsCQRS
{
	public interface IDogadjajWriteRepository
	{
		Task<int> AddStrucniDogadjaj(StrucniDogadjaj.Domain.Write.StrucniDogadjaj dogadjaj);
		Task<int> EditStrucniDogadjaj(StrucniDogadjaj.Domain.Write.StrucniDogadjaj dogadjaj);
		Task<int> DeleteStrucniDogadjaj(int id);

	}
}
