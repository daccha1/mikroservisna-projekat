using EventActivityService.Repositories.SQL_Server;
using Microsoft.AspNetCore.Mvc;

namespace EventActivityService.Controllers
{
	[ApiController]
	[Route("{controller}")]
	public class EventActivityController
	{
		EventActivitySQLRepository _repo;
		public EventActivityController(EventActivitySQLRepository repo)
		{
			
		}

		[HttpPost]
		public IActionResult GuestCheckIn([FromBody] Guid GuestId)
		{

		}
	}
}
