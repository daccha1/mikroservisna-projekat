using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.Models;
using mikroservisnaApp.Repositories.SQL_Server;

namespace mikroservisnaApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class StrucniDogadjajController : ControllerBase
	{
		private IStrucniDogadjaj _repository;
		public StrucniDogadjajController(IStrucniDogadjaj repository)
		{
			_repository = repository;
		}

		[HttpGet("events")]
		public async Task<ActionResult<List<StrucniDogadjaj>>> GetAll()
		{
			return Ok(await _repository.GetAll());
		}

	}
}
