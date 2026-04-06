using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.LokacijaDTO;
using Polly;
using System.Diagnostics;

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

			var retryPolicy = Polly.Policy
							  .Handle<HttpRequestException>() // http error
							  .Or<TaskCanceledException>()    // timeout
							  .WaitAndRetryAsync(2, retry =>
							  {
								  Debug.Write($">>>>>>> Current retry: {retry}");
								  return TimeSpan.FromSeconds(1);
							  });

			HttpResponseMessage httpResponse = null;
			var client = HttpFactory.CreateClient("LokacijaAPI");

			httpResponse = await retryPolicy.ExecuteAsync<HttpResponseMessage>( async () =>
			{
				var results = await client.GetAsync("/lokacija");
				results.EnsureSuccessStatusCode();
				return results;
			});

			List<LokacijaResponseDTO>? lokacije = await httpResponse.Content.ReadFromJsonAsync<List<LokacijaResponseDTO>>();
			
			return lokacije;
		}


		public async Task<LokacijaResponseDTO> GetById(int idLocation)
		{

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
