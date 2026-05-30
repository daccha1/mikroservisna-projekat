using EventActivityService.Models.Domain_models;
using EventActivityService.Repositories.SQL_Server;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EventActivityService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EventActivityController : Controller
	{
		EventActivitySQLRepository _repo;
		public EventActivityController(EventActivitySQLRepository repo)
		{
			_repo = repo;
		}

		[HttpPost("checkin")]
		public async Task<ActionResult> GuestCheckIn([FromBody] Guid guestId)
		{
			var activity = await _repo.CheckIn(guestId);
			if(activity == null)
			{
				return Problem();
			}
			return Ok(activity);
		}

		[HttpPost("checkout")]
		public async Task<ActionResult> GuestCheckOut([FromBody] Guid guestId)
		{
			var activity = await _repo.CheckOut(guestId);
			if (activity == null)
			{
				return Problem();
			}
			return Ok(activity);
		}

		[HttpPost("switch-hall/{guestId}/{hallNumber}")]
		public async Task<IActionResult> SwitchHall([FromRoute] Guid guestId, [FromRoute] int hallNumber)
		{
			var activity = await _repo.SwitchHall(guestId, hallNumber);
			if (activity == null) return Problem();
			return Ok();
		}

		[HttpPost("add-balance/{guestId}/{balance}")]
		public async Task<IActionResult> AddBalance([FromRoute] Guid guestId, [FromRoute] decimal balance)
		{
			var activity = await _repo.AddBalance(guestId, balance);
			return Ok();
		}

		[HttpPost("remove-balance/{guestId}/{balance}")]
		public async Task<IActionResult> RemoveBalance([FromRoute] Guid guestId, [FromRoute] decimal balance)
		{
			var activity = await _repo.RemoveBalance(guestId, balance*(-1));
			return Ok();
		}

		[HttpGet("guest/{guestId}")]
		public async Task<IActionResult> GetGuestActivity([FromRoute] Guid guestId)
		{
			var activity = await _repo.Load<EventActivity>(guestId);
			return Ok(activity);
		}

		[HttpGet("guest-events/{guestId}")]
		public async Task<IActionResult> GetGuestEvents([FromRoute] Guid guestId)
		{
			var events = await _repo.GetAllEvents(guestId);
			if(events != null)
			{
				return Ok(events);
			}
			return NotFound();
		}

	}
}
