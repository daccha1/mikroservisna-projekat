using ContractsCQRS;
using mikroservisnaApp.CQRS_Container.Application.Repositories;
using System.Numerics;

namespace mikroservisnaApp.CQRS_Container.Application.Commands
{
	public class AddDogadjajCommandHandler
	{

		public IDogadjajWriteRepository _repository;
		public AddDogadjajCommandHandler(IDogadjajWriteRepository repo)
		{
			_repository = repo;
		}

		public async Task<int> Handle(AddDogadjajCommand cmd)
		{	
			// konverzija podataka

			var dogadjaj = new StrucniDogadjaj.Domain.Write.StrucniDogadjaj()
			{
				Agenda = cmd.Agenda,
				Cena = cmd.Cena,
				DatumVreme = cmd.DatumVreme,
				LokacijaId = cmd.LokacijaId,
				Naziv = cmd.Naziv,
				Trajanje = cmd.Trajanje,
				TipId = cmd.TipId,
				OrganizatorId = cmd.OrganizatorId
			};

			int eventId = await _repository.AddStrucniDogadjaj(dogadjaj);

			return eventId;
		}

	}
}
