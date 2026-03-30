using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO;
using mikroservisnaApp.Models.DTO.StrucniDogadjajDTO;
using mikroservisnaApp.Repositories.SQL_Server;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace mikroservisnaApp.Controllers
{
	[ApiController]
	[Route("events")]
	public class StrucniDogadjajController : ControllerBase
	{
		private IStrucniDogadjaj _repository;
		public StrucniDogadjajController(IStrucniDogadjaj repository)
		{
			_repository = repository;
		}

		[HttpGet("")]
		public async Task<IActionResult> GetAll()
		{
			var events = await _repository.GetAll();
			if(events.Count == 0)
			{
				return NotFound("Trenutno nema dogadjaja.");
			}
			return Ok(events);
		}


		[HttpGet("{id}")]
	
		public async Task<IActionResult> GetById(int id)
		{
			var eventDetails = await _repository.GetById(id);
			if(eventDetails == null)
			{
				return NotFound("Nema datog dogadjaja");
			}
			return Ok(eventDetails);
		}

		[HttpPost("add")]
		public async Task<IActionResult> Post([FromBody] StrucniDogadjajRequestDTO addEvent)
		{
			var newEvent = await _repository.Post(addEvent);
			if(newEvent == null)
			{
				return NotFound("Nije uspelo dodavanje dogadjaja.");
			}
			return Ok(newEvent);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] StrucniDogadjajRequestDTO newData)
		{
			var update = await _repository.Update(id, newData);
			if(update == false)
			{
				return BadRequest("Nije moguce izvrsiti update: objekat ne postoji ili su podaci nevalidni");
			}
			return Ok();
		}

	}
}
