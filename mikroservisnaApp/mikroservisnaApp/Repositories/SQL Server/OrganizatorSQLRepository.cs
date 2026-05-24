using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.OrganizatorDTO;
using Polly;
using System.Diagnostics;
using Common.EventService;

namespace mikroservisnaApp.Repositories.SQL_Server
{
	public class OrganizatorSQLRepository : IOrganizator
	{
		private DogadjajiDbContext context;
		private IHttpClientFactory HttpFactory { get; }
		public OrganizatorSQLRepository(DogadjajiDbContext context, IHttpClientFactory httpClientFactory)
		{
			this.context = context;
			HttpFactory = httpClientFactory;
		}

		public async Task<List<OrganizatorResponseDTO>> GetAll()
		{
			// HttpRequestException : tip exceptiona koji baca HttpClient
			var policyHandler = Polly.Policy
								.Handle<HttpRequestException>()
								.Or<TaskCanceledException>()
								.WaitAndRetryAsync(2, retry => 
								{
									Debug.WriteLine($">>>>>>>>>>>>   Current retry: {retry}");
									Console.Beep(); // obrisi ovo posle 
									return TimeSpan.FromSeconds(1);
								});
			// policy koji hendluje bacene HttpRequestException-e tako da na svaki ex tog tipa koji je neuspesan vrsi
			// do navedenog broja retry-a, a izmedju svakog retry ceka 1s
			// (ako predje navedeni broj retry-a baca exception dalje)

			// testirano na timeout i kada je ugasen server (radi)

			HttpResponseMessage httpResponse = null;
			var client = HttpFactory.CreateClient("OrganizatorAPI");
			
			httpResponse = await policyHandler.ExecuteAsync<HttpResponseMessage>(async () =>
			{
				httpResponse = await client.GetAsync("/organizator");
				httpResponse.EnsureSuccessStatusCode(); // ukoliko nije Success status code baca ex.
				return httpResponse;
			});

			var listaOrganizatora = await httpResponse.Content.ReadFromJsonAsync<List<OrganizatorResponseDTO>>();
			
			return listaOrganizatora;
		}

		public async Task<OrganizatorResponseDTO> GetById(int idOrganizator)
		{
			var organizator = await context.Organizatori
				.Where(o => o.Id == idOrganizator)
				.Select(o => new OrganizatorResponseDTO
				{
					Id = o.Id,
					Ime = o.Ime,
					Prezime = o.Prezime,
					ListaDogadjaja = o.ListaDogadjaja.Select(d => d.Naziv).ToList()
				}).FirstOrDefaultAsync();

			return organizator;
		}

		public async Task<OrganizatorRequestDTO> Post(OrganizatorRequestDTO organizator)
		{
			Organizator noviOrganizator = new()
			{
				Ime = organizator.Ime,
				Prezime = organizator.Prezime
			};

			context.Organizatori.Add(noviOrganizator);
			int isAdded = context.SaveChanges();
			if (isAdded != 0)
			{
				return organizator;
			}
			return null;
		}

		public async Task<bool> Update(int idOrganizator, OrganizatorRequestDTO updatedOrganizator)
		{
			var organizator = await context.Organizatori
				.Where(o => o.Id == idOrganizator)
				.FirstOrDefaultAsync();

			if (organizator == null)
			{
				return false;
			}

			organizator.Ime = updatedOrganizator.Ime;
			organizator.Prezime = updatedOrganizator.Prezime;

			context.SaveChanges();
			return true;
		}
	}
}
