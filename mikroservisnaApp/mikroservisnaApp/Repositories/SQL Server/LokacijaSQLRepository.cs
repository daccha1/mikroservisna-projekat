using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.LokacijaDTO;
using mikroservisnaApp.Patterns;
using Polly;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace mikroservisnaApp.Repositories.SQL_Server
{
	public class LokacijaSQLRepository : ILokacija
	{
		private DogadjajiDbContext _context;
		private IHttpClientFactory _HttpFactory { get; }
		private CircuitBreaker _breaker;

		public LokacijaSQLRepository(DogadjajiDbContext context, IHttpClientFactory clientFactory, CircuitBreaker cb)
		{
			this._context = context;
			_HttpFactory = clientFactory;
			_breaker = cb;
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
			var client = _HttpFactory.CreateClient("LokacijaAPI");

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

			var retryPolicy = Polly.Policy
							  .Handle<HttpRequestException>()
							  .Or<TaskCanceledException>()
							  .WaitAndRetryAsync(2, retry =>
							  {
								  Debug.WriteLine($"\n>>>>>>>>> Current retry: {retry}");
								  return TimeSpan.FromSeconds(1);
							  });


			var client = _HttpFactory.CreateClient("LokacijaAPI");

			httpResponse = await retryPolicy.ExecuteAsync<HttpResponseMessage>(async () =>
			{
				try
				{
					return await _breaker.ExecuteAsync<HttpResponseMessage>(async () =>
							{
								var result = await client.GetAsync($"/lokacija/{idLocation}");
								result.EnsureSuccessStatusCode();
								return result;
							});
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Servis zaduzen za LOKACIJE trenutno nije dostupan");
					return null;
				}
			});

			if(httpResponse == null)
			{
				return null;
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

			_context.Lokacije.Add(lokacija);
			int isAdded = _context.SaveChanges();
			if(isAdded != 0)
			{
				return location;
			}
			return null;
		}

		public async Task<bool> Update(int idLocation, LokacijaRequestDTO updatedLocation)
		{
			var location = await _context.Lokacije.Where(l => l.Id == idLocation).FirstOrDefaultAsync();

			if(location == null)
			{
				return false;
			}

			location.Naziv = updatedLocation.Naziv;
			location.Adresa = updatedLocation.Adresa;
			location.Kapacitet = updatedLocation.Kapacitet;

			_context.SaveChanges();
			return true;

		}
	}
}
