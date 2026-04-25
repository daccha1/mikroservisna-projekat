using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.StrucniDogadjajDTO;
using mikroservisnaApp.MQ_Container;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text;

namespace mikroservisnaApp.Repositories.SQL_Server
{
	public class StrucniDogadjajSQLRepository : IStrucniDogadjaj
	{

		private DogadjajiDbContext context;
		private IMQClient _mqClient;
		
		public StrucniDogadjajSQLRepository(DogadjajiDbContext context, IMQClient mqPublisher)
		{
			this.context = context;
			this._mqClient = mqPublisher;
		}

		public async Task<List<StrucniDogadajajResponseDTO>> GetAll()
		{
			var events = await context.Dogadjaji
				//.Include(d => d.Lokacija)
				//.Include(d => d.Organizator)
				//.Include(d => d.Tip)
				.Select(d => new StrucniDogadajajResponseDTO
				{
					Naziv = d.Naziv,
					Agenda = d.Agenda,
					Cena = d.Cena,
					Id = d.Id,
					DatumVreme = d.DatumVreme,
					NazivLokacije = d.Lokacija.Naziv,
					NazivOrganizatora = d.Organizator.Ime + ' ' + d.Organizator.Prezime,
					NazivTipa = d.Tip.Naziv,
					Trajanje = d.Trajanje
				}).ToListAsync();

			return events;
		}

		public async Task<StrucniDogadjajDetailsResponseDTO> GetById(int idEvent)
		{
			var eventDetails = await context.Dogadjaji.Where(d => d.Id == idEvent)
												//.Include(d => d.Tip)
												//.Include(d => d.Lokacija)
												//.Include(d => d.SpisakPredavaca)
												//.Include(d => d.Organizator)
												.Select(d => new StrucniDogadjajDetailsResponseDTO
												{
													Naziv = d.Naziv,
													Agenda = d.Agenda,
													Cena = d.Cena,
													Id = d.Id,
													DatumVreme = d.DatumVreme,
													NazivLokacije = d.Lokacija.Naziv,
													NazivOrganizatora = d.Organizator.Ime + " " + d.Organizator.Prezime,
													NazivTipa = d.Tip.Naziv,
													Trajanje = d.Trajanje,
													Predavaci = d.SpisakPredavaca.Select(p => p.Predavac.Ime + " " + p.Predavac.Prezime).ToList()
												}).FirstOrDefaultAsync();

			return eventDetails;
		}

		public async Task<bool> validateEvent(StrucniDogadjajRequestDTO dogadjaj)
		{
			// lokacijaId, organizatorId, predavacId, tipId
			var location = context.Lokacije.ToList().Exists(l => l.Id == dogadjaj.LokacijaId);
			var organizator = context.Lokacije.ToList().Exists(o => o.Id == dogadjaj.OrganizatorId);
			var tip = context.Lokacije.ToList().Exists(t => t.Id == dogadjaj.TipId);
			var predavac = true;

			foreach(var p in dogadjaj.Predavaci)
			{
				var postojiPredavac = context.Lokacije.ToList().Exists(pr => pr.Id == p.PredavacId);
				if(postojiPredavac == false)
				{
					predavac = false;
					break;
				}
			}

			if(location == false || organizator == false || tip == false || predavac == false)
			{
				return false;
			}
			return true;
		}
		// validacija: ako neki od ID-eva za koje je ovaj dogadjaj vezan ne postoje
		//														     vratiti gresku
		// (slucaj da administrator sam unosi id-eve u tekst polja)

		//bool validationResult = await validateEvent(dogadjaj);
		//if (!validationResult)
		//{
		//	return null; // kontroler da proveri null
		//}

		// dogadjaj -> extract lokacija & organizator IDs -> MQ ka LocationAPI & 
		public async Task<StrucniDogadjajRequestDTO> Post(StrucniDogadjajRequestDTO dogadjaj)
		{

			bool locationExists = false;
			bool organizatorExists = false;

            #region locationHandler
            string lokacijaId = Convert.ToString(dogadjaj.LokacijaId);
			var signal = new SemaphoreSlim(0, 1);
            EventHandler<string> handler = null;
            handler = (_, args) =>
            {
                if(args == "true")
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
				return null;
			}
            #endregion


            string organizatorId = Convert.ToString(dogadjaj.OrganizatorId);
			await _mqClient.SendMessageAsync(organizatorId, organizatorExchangeName, organizatorRoutingKey, organizatorConsumeKey);
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

            await signal.WaitAsync(TimeSpan.FromSeconds(3));

            if (!organizatorExists)
			{
				return null;
			}

            StrucniDogadjaj eventToAdd = new()
			{
				Agenda = dogadjaj.Agenda,
				Cena = dogadjaj.Cena,
				DatumVreme = dogadjaj.DatumVreme,
				Naziv = dogadjaj.Naziv,
				Trajanje = dogadjaj.Trajanje,
				LokacijaId = dogadjaj.LokacijaId,
				OrganizatorId = dogadjaj.OrganizatorId,
				TipId = dogadjaj.TipId,
				SpisakPredavaca = dogadjaj.Predavaci.Select(p => new DogadjajPredavac
				{
					PredavacId = p.PredavacId,
					RasporedPredavanja = p.RasporedPredavanja,
				}).ToList()
			};

			int successful = 0;
			try
			{
				var isAdded = await context.Dogadjaji.AddAsync(eventToAdd);
				successful = await context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
                Console.WriteLine( ex.Message);
				throw;
			}
			if(successful != 0)
			{
				return dogadjaj;
			}
			return null;

		}

		public StrucniDogadjaj updateEvent(StrucniDogadjajRequestDTO updatedEvent, StrucniDogadjaj updateEvent)
		{
			updateEvent.Naziv = updatedEvent.Naziv;
			updateEvent.Agenda = updatedEvent.Agenda;
			updateEvent.Trajanje = updatedEvent.Trajanje;
			updateEvent.Cena = updatedEvent.Cena;
			updateEvent.DatumVreme = updatedEvent.DatumVreme;
			updateEvent.OrganizatorId = updatedEvent.OrganizatorId;
			updateEvent.LokacijaId = updatedEvent.LokacijaId;
			updateEvent.TipId = updatedEvent.TipId;
			updateEvent.SpisakPredavaca = updatedEvent.Predavaci.Select(p => new DogadjajPredavac
			{
				PredavacId = p.PredavacId,
				StrucniDogadjajId = updateEvent.Id,
				RasporedPredavanja = p.RasporedPredavanja
			}).ToList();

			return updateEvent;
		}
		public async Task<bool> Update(int idEvent, StrucniDogadjajRequestDTO updatedEvent)
		{
			StrucniDogadjaj eventToUpdate = await context.Dogadjaji.Where(d => d.Id == idEvent).FirstAsync();
			
			if(updateEvent == null)
			{
				return false;
			}

			bool isValid = await validateEvent(updatedEvent); 
			if (!isValid) return false;
			
			eventToUpdate = updateEvent(updatedEvent, eventToUpdate);

			context.SaveChanges();

			return true;
		}


		// email
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
