using Microsoft.AspNetCore.Mvc;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Models.DTO.PredavacDTO;

namespace mikroservisnaApp.Controllers
{
	[ApiController]
	[Route("speakers")]
	public class PredavaciController : ControllerBase
	{
		private IPredavac _repository;

		public PredavaciController(IPredavac repo)
		{
			_repository = repo;
		}

		[HttpGet("")]
		public async Task<IActionResult> GetAll()
		{
			var predavaci = await _repository.GetAll();
			if (predavaci == null)
			{
				return NotFound("Nema predavaca.");
			}
			return Ok(predavaci);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var predavac = await _repository.GetById(id);
			if (predavac == null)
			{
				return NotFound("Ne postoji dati predavac.");
			}
			return Ok(predavac);
		}

		[HttpPost("add")]
		public async Task<IActionResult> Post([FromBody] PredavacRequestDTO predavacToAdd)
		{
			var isAdded = await _repository.Post(predavacToAdd);
			if (isAdded == null)
			{
				return BadRequest("Nije moguce dodati dati objekat.");
			}
			return Ok(isAdded);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] PredavacRequestDTO predavacToUpdate)
		{
			var successful = await _repository.Update(id, predavacToUpdate);
			if (!successful)
			{
				return BadRequest("Nije moguce izvrsiti update.");
			}
			return Ok();
		}
	}
}
