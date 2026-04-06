using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.LokacijaDTO;

namespace mikroservisnaApp.Repositories.SQL_Server
{
	public class LokacijaSQLRepository : ILokacija
	{
		private DogadjajiDbContext context;
		private IHttpClientFactory HttpFactory { get; }

		public LokacijaSQLRepository(DogadjajiDbContext context, IHttpClientFactory clientFactory)
		{
			this.context = context;
			HttpFactory = clientFactory;
		}

		//public async Task<List<LokacijaResponseDTO>> GetAll()
		//{
		//	var locations = await context.Lokacije.Select(l => new LokacijaResponseDTO
		//	{
		//		Id = l.Id,
		//		Naziv = l.Naziv,
		//		Adresa = l.Adresa,
		//		Kapacitet = l.Kapacitet,
		//		SpisakDogadjaja = l.ListaDogadjaja.Select(d => $"{d.Naziv}").ToList()
		//	}).ToListAsync();

		//	if(locations == null)
		//	{
		//		return null;
		//	}

		//	return locations;
		//}

		public async Task<List<LokacijaResponseDTO>> GetAll()
		{
			HttpResponseMessage httpResponse = null;

			var client = HttpFactory.CreateClient("LokacijaAPI");

			httpResponse = await client.GetAsync("/lokacija");

			if(httpResponse == null || !httpResponse.IsSuccessStatusCode)
			{
				return new List<LokacijaResponseDTO>();
			}

			List<LokacijaResponseDTO>? lokacije = await httpResponse.Content.ReadFromJsonAsync<List<LokacijaResponseDTO>>();
			
			return lokacije;

		}


		public async Task<LokacijaResponseDTO> GetById(int idLocation)
		{
			//var location = await context.Lokacije
			//					.Include(l => l.ListaDogadjaja)
			//					.Where(l => l.Id == idLocation)
			//					.FirstOrDefaultAsync();

			//if(location == null)
			//{
			//	return null;
			//}

			//LokacijaResponseDTO dto = new()
			//{
			//	Id = location.Id,
			//	Adresa = location.Adresa,
			//	Kapacitet = location.Kapacitet,
			//	Naziv = location.Naziv,
			//	SpisakDogadjaja = location?.ListaDogadjaja?.Select(d => $"{d.Naziv}").ToList()
			//};

			//return dto;

			HttpResponseMessage httpResponse = null;

			var client = HttpFactory.CreateClient("LokacijaAPI");

			httpResponse = await client.GetAsync($"/lokacija/{idLocation}");

			if (httpResponse == null || !httpResponse.IsSuccessStatusCode)
			{
				return new LokacijaResponseDTO();
			}

			LokacijaResponseDTO lokacija = await httpResponse.Content.ReadFromJsonAsync<LokacijaResponseDTO>();

			return lokacija;
		}

		public async Task<LokacijaRequestDTO> Post(LokacijaRequestDTO location)
		{
			Lokacija lokacija = new()
			{
				Adresa = location.Adresa,
				Kapacitet = location.Kapacitet,
				Naziv = location.Naziv
			};

			context.Lokacije.Add(lokacija);
			int isAdded = context.SaveChanges();
			if(isAdded != 0)
			{
				return location;
			}
			return null;
		}

		public async Task<bool> Update(int idLocation, LokacijaRequestDTO updatedLocation)
		{
			var location = await context.Lokacije.Where(l => l.Id == idLocation).FirstOrDefaultAsync();

			if(location == null)
			{
				return false;
			}

			location.Naziv = updatedLocation.Naziv;
			location.Adresa = updatedLocation.Adresa;
			location.Kapacitet = updatedLocation.Kapacitet;

			context.SaveChanges();
			return true;

		}
	}
}
