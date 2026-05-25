using ContractsCQRS;
using Polly;

namespace mikroservisnaApp.CQRS_Container.Application.Commands
{
	public class EditDogadjajCommandHandler
	{
		public IDogadjajWriteRepository _repository;
		
		public EditDogadjajCommandHandler(IDogadjajWriteRepository repo)
		{
			_repository = repo;
		}

		public async Task<int> Handle(EditDogadjajCommand dogadjajToUpdate)
		{
			StrucniDogadjaj.Domain.Write.StrucniDogadjaj newEvent = new()
			{
				Id = dogadjajToUpdate.Id,
				Agenda = dogadjajToUpdate.Agenda,
				Cena = dogadjajToUpdate.Cena,
				DatumVreme = dogadjajToUpdate.DatumVreme,
				LokacijaId = dogadjajToUpdate.LokacijaId,
				Naziv = dogadjajToUpdate.Naziv,
				OrganizatorId = dogadjajToUpdate.OrganizatorId,
				TipId = dogadjajToUpdate.TipId,
				Trajanje = dogadjajToUpdate.Trajanje
			};

			int result = await _repository.EditStrucniDogadjaj(newEvent);

			if (result == -1) return -1;
			return 1;

		}

	}
}
