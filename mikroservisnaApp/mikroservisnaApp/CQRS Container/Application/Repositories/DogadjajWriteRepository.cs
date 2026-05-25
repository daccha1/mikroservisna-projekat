using Common;
using Common.EventService;
using Common.StrucniDogadjajDTO;
using ContractsCQRS;
using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Data;
using mikroservisnaApp.MQ_Container;
using Polly;
using StrucniDogadjaj.Infrastructure.Write.EFCore.Data;
using System.Text.Json;

namespace mikroservisnaApp.CQRS_Container.Application.Repositories
{
	public class DogadjajWriteRepository : IDogadjajWriteRepository
	{
		private DogadjajiDbContext primaryContext;
		private WriteDbContext context;
		private IMQClient _mqClient;
		public DogadjajWriteRepository(WriteDbContext ctx, IMQClient mqPublisher, DogadjajiDbContext primContext)
		{
			context = ctx;
			_mqClient = mqPublisher;
			primaryContext = primContext;
		}

		public async Task<int> AddStrucniDogadjaj(StrucniDogadjaj.Domain.Write.StrucniDogadjaj dogadjaj)
		{	
			bool locationExists = false;
			bool organizatorExists = false;

			#region locationHandler
			string lokacijaId = Convert.ToString(dogadjaj.LokacijaId);
			var signal = new SemaphoreSlim(0, 1);
			EventHandler<string> handler = null;
			handler = (_, args) =>
			{
				if (args == "true")
				{
					locationExists = true;
					signal.Release();
				}

				_mqClient.OdgovorPrimljenLocation -= handler; // ukloni sebe
			};
			_mqClient.OdgovorPrimljenLocation += handler;



			await _mqClient.SendMessageAsync(lokacijaId, locationExchangeName, locationRoutingKey, replyToString: locationConsumeKey);

			await signal.WaitAsync(TimeSpan.FromSeconds(3));

			if (!locationExists)
			{
				return -1;
			}
			#endregion


			string organizatorId = Convert.ToString(dogadjaj.OrganizatorId);
			EventHandler<string> handlerOrganizator = null;
			handlerOrganizator = (_, args) =>
			{
				if (args == "true")
				{
					organizatorExists = true;
					signal.Release();
				}

				_mqClient.OdgovorPrimljenOrganizator -= handlerOrganizator; // ukloni sebe
			};
			_mqClient.OdgovorPrimljenOrganizator += handlerOrganizator;

			await _mqClient.SendMessageAsync(organizatorId, organizatorExchangeName, organizatorRoutingKey, replyToString: organizatorConsumeKey);


			await signal.WaitAsync(TimeSpan.FromSeconds(3));

			if (!organizatorExists)
			{
				return -1;
			}

			int successful = 0;
			using var transaction = await context.Database.BeginTransactionAsync();
			try
			{
				await context.Dogadjaji.AddAsync(dogadjaj);
				Console.WriteLine(">>>> DODAVANJE DOGADJAJA!");
				successful = await context.SaveChangesAsync();
				Console.WriteLine(">>>> USPESNO DODAVANJE!");
				
				// Posto outbox tabela prima StrucniDogadjajDTO, moramo da serijalizujemo dogadjaj u taj format
				// da bi dogadjaj bio sacuvan u outbox tabeli

				StrucniDogadjajRequestDTO tempDTO = new()
				{
					Agenda = dogadjaj.Agenda,
					OrganizatorId = dogadjaj.OrganizatorId,
					Cena = dogadjaj.Cena,
					DatumVreme = dogadjaj.DatumVreme,
					LokacijaId = dogadjaj.LokacijaId,
					Naziv = dogadjaj.Naziv,
					TipId = dogadjaj.TipId,
					Trajanje = dogadjaj.Trajanje
				};

				OutboxMessage outboxMsg = new()
				{
					Payload = JsonSerializer.Serialize<StrucniDogadjajRequestDTO>(tempDTO),
					Event = OperationEvent.Created
				};

				await primaryContext.OutboxTable.AddAsync(outboxMsg);
				successful += await context.SaveChangesAsync();
				Console.WriteLine("OUTBOX SACUVAN!");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				Console.WriteLine(ex.Message);
				throw;
			}
			return dogadjaj.Id;
		}

		public async Task<int> EditStrucniDogadjaj(StrucniDogadjaj.Domain.Write.StrucniDogadjaj dogadjaj)
		{
			
			try
			{
				StrucniDogadjaj.Domain.Write.StrucniDogadjaj eventToUpdate = await context.Dogadjaji.Where(d => d.Id == dogadjaj.Id).FirstAsync();

				eventToUpdate.Naziv = dogadjaj.Naziv;
				eventToUpdate.Agenda = dogadjaj.Agenda;
				eventToUpdate.Trajanje = dogadjaj.Trajanje;
				eventToUpdate.LokacijaId = dogadjaj.LokacijaId;
				eventToUpdate.Cena = dogadjaj.Cena;
				eventToUpdate.DatumVreme = dogadjaj.DatumVreme;
				eventToUpdate.OrganizatorId = dogadjaj.OrganizatorId;
				eventToUpdate.TipId = dogadjaj.TipId;

				context.Update(eventToUpdate);
				await context.SaveChangesAsync();
				return 1;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Update Event: Greska! Nije uspelo dodavanje " + ex.Message);
				return -1;
			}

		}

		public Task<int> DeleteStrucniDogadjaj(int id)
		{
			throw new NotImplementedException();
		}

		string emailExchangeName = "events.event.eventsExchange";
		string emailQueueName = "events.event.publishQueue";
		string emailRoutingKey = "event-publish-key";

		// location
		string locationExchangeName = "events.location.locationExchange";
		string locationQueueName = "events.location.publishQueue";
		string locationRoutingKey = "location-publish-key";

		string locationConsumeQueue = "events.location.consumeQueue";
		string locationConsumeKey = "location-consume-key";

		// organizator
		string organizatorExchangeName = "events.organizer.organizerExchange";
		string organizatorQueueName = "events.organizer.organizerQueue";
		string organizatorRoutingKey = "organizer-publish-key";

		string organizatorConsumeQueue = "events.organizer.consumeQueue";
		string organizatorConsumeKey = "organizer-consume-key";

	}
}
