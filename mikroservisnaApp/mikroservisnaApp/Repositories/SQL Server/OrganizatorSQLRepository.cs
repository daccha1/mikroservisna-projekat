using Microsoft.EntityFrameworkCore;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Data;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO.OrganizatorDTO;

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
			//var organizatori = await context.Organizatori.Select(o => new OrganizatorResponseDTO
			//{
			//	Id = o.Id,
			//	Ime = o.Ime,
			//	Prezime = o.Prezime,
			//	ListaDogadjaja = o.ListaDogadjaja.Select(d => d.Naziv).ToList()
			//}).ToListAsync();

			//return organizatori;

			HttpResponseMessage httpResposne = null;
			var client = HttpFactory.CreateClient("OrganizatorAPI");
			
			httpResposne = await client.GetAsync("/organizator");

			if(httpResposne == null || !httpResposne.IsSuccessStatusCode)
			{
				return new List<OrganizatorResponseDTO>();
			}

			var listaOrganizatora = await httpResposne.Content.ReadFromJsonAsync<List<OrganizatorResponseDTO>>();
			
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
