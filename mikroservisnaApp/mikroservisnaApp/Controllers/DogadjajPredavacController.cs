using Microsoft.AspNetCore.Mvc;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Models.DTO.DogadjajPredavacDTO;

namespace mikroservisnaApp.Controllers
{
	[ApiController]
	[Route("event-speakers")]
	public class DogadjajPredavacController : ControllerBase
	{
		private IDogadjajPredavac _repository;

		public DogadjajPredavacController(IDogadjajPredavac repo)
		{
			_repository = repo;
		}

		[HttpGet("")]
		public async Task<IActionResult> GetAll()
		{
			var stavke = await _repository.GetAll();
			if (stavke == null)
			{
				return NotFound("Nema stavki dogadjaj-predavac.");
			}
			return Ok(stavke);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var stavka = await _repository.GetById(id);
			if (stavka == null)
			{
				return NotFound("Ne postoji data stavka dogadjaj-predavac.");
			}
			return Ok(stavka);
		}

		[HttpPost("add")]
		public async Task<IActionResult> Post([FromBody] DogadjajPredavacRequestDTO stavkaToAdd)
		{
			var isAdded = await _repository.Post(stavkaToAdd);
			if (isAdded == null)
			{
				return BadRequest("Nije moguce dodati dati objekat. Proverite da li PredavacId i StrucniDogadjajId postoje.");
			}
			return Ok(isAdded);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] DogadjajPredavacRequestDTO stavkaToUpdate)
		{
			var successful = await _repository.Update(id, stavkaToUpdate);
			if (!successful)
			{
				return BadRequest("Nije moguce izvrsiti update.");
			}
			return Ok();
		}
	}
}
