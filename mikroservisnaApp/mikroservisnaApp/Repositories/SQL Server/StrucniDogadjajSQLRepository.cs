using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.StrucniDogadjajDTO;
using System.ComponentModel;

namespace mikroservisnaApp.Repositories.SQL_Server
{
	public class StrucniDogadjajSQLRepository : IStrucniDogadjaj
	{

		private DogadjajiDbContext context;

		public StrucniDogadjajSQLRepository(DogadjajiDbContext context)
		{
			this.context = context;
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

		public async Task<StrucniDogadjajRequestDTO> Post(StrucniDogadjajRequestDTO dogadjaj)
		{
			// validacija: ako neki od ID-eva za koje je ovaj dogadjaj vezan ne postoje
			//														     vratiti gresku
			// (slucaj da administrator sam unosi id-eve u tekst polja)

			bool validationResult = await validateEvent(dogadjaj);
			if (!validationResult)
			{
				return null; // kontroler da proveri null
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

			var isAdded = context.Dogadjaji.Add(eventToAdd);
			int successful = context.SaveChanges();
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
	}
}
