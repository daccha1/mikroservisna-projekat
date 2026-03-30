using Microsoft.AspNetCore.Mvc;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Models.DTO.OrganizatorDTO;

namespace mikroservisnaApp.Controllers
{
	[ApiController]
	[Route("organizers")]
	public class OrganizatoriController : ControllerBase
	{
		private IOrganizator _repository;

		public OrganizatoriController(IOrganizator repo)
		{
			_repository = repo;
		}

		[HttpGet("")]
		public async Task<IActionResult> GetAll()
		{
			var organizatori = await _repository.GetAll();
			if (organizatori == null)
			{
				return NotFound("Nema organizatora.");
			}
			return Ok(organizatori);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var organizator = await _repository.GetById(id);
			if (organizator == null)
			{
				return NotFound("Ne postoji dati organizator.");
			}
			return Ok(organizator);
		}

		[HttpPost("add")]
		public async Task<IActionResult> Post([FromBody] OrganizatorRequestDTO organizatorToAdd)
		{
			var isAdded = await _repository.Post(organizatorToAdd);
			if (isAdded == null)
			{
				return BadRequest("Nije moguce dodati dati objekat.");
			}
			return Ok(isAdded);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] OrganizatorRequestDTO organizatorToUpdate)
		{
			var successful = await _repository.Update(id, organizatorToUpdate);
			if (!successful)
			{
				return BadRequest("Nije moguce izvrsiti update.");
			}
			return Ok();
		}
	}
}
