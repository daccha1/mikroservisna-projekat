using Microsoft.AspNetCore.Mvc;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Models.DTO.TipDogadjajaDTO;

namespace mikroservisnaApp.Controllers
{
	[ApiController]
	[Route("event-types")]
	public class TipDogadjajaController : ControllerBase
	{
		private ITipDogadjaja _repository;

		public TipDogadjajaController(ITipDogadjaja repo)
		{
			_repository = repo;
		}

		[HttpGet("")]
		public async Task<IActionResult> GetAll()
		{
			var tipovi = await _repository.GetAll();
			if (tipovi == null)
			{
				return NotFound("Nema tipova dogadjaja.");
			}
			return Ok(tipovi);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var tip = await _repository.GetById(id);
			if (tip == null)
			{
				return NotFound("Ne postoji dati tip dogadjaja.");
			}
			return Ok(tip);
		}

		[HttpPost("add")]
		public async Task<IActionResult> Post([FromBody] TipDogadjajaRequestDTO tipToAdd)
		{
			var isAdded = await _repository.Post(tipToAdd);
			if (isAdded == null)
			{
				return BadRequest("Nije moguce dodati dati objekat.");
			}
			return Ok(isAdded);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] TipDogadjajaRequestDTO tipToUpdate)
		{
			var successful = await _repository.Update(id, tipToUpdate);
			if (!successful)
			{
				return BadRequest("Nije moguce izvrsiti update.");
			}
			return Ok();
		}
	}
}
