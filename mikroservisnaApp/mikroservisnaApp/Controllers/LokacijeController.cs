using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Models.DTO.LokacijaDTO;
using mikroservisnaApp.Repositories.SQL_Server;

namespace mikroservisnaApp.Controllers
{
	[ApiController]
	[Route("locations")]
	public class LokacijeController : ControllerBase
	{

		private ILokacija _repository;

		public LokacijeController(ILokacija repo)
		{
			_repository = repo;
		}
		

		[HttpGet("")]
		public async Task<IActionResult> GetAll()
		{
			var locations = await _repository.GetAll();

			if(locations == null)
			{
				return NotFound("Nema lokacija");
			}
			return Ok(locations);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var location = await _repository.GetById(id);
			if(location == null)
			{
				return NotFound("Ne postoji ta lokacija.");
			}
			return Ok(location);
		}

		[HttpPost("add")]
		public async Task<IActionResult> Post([FromBody] LokacijaRequestDTO lokacijaToAdd)
		{
			var isAdded = await _repository.Post(lokacijaToAdd);
			if(isAdded == null)
			{
				return BadRequest("Nije moguce dodati dati objekat");
			}
			return Ok(isAdded);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] LokacijaRequestDTO lokacijaToAdd)
		{
			var successful = await _repository.Update(id, lokacijaToAdd);
			if (!successful)
			{
				return BadRequest("Nije moguce izvrsiti update");
			}
			return Ok();
		}

	}
}
