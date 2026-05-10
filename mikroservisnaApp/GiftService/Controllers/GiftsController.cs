using GiftService.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GiftService.Controllers
{
	[ApiController]
	[Route("{controller}")]
	public class GiftsController : Controller
	{
		private IGift _repository;
		public GiftsController(IGift repo)
		{
			_repository = repo;
		}

		[HttpGet]
		public ActionResult GetAll()
		{
			return Ok(_repository.GetGifts());
		}

		[HttpGet]
		[Route("{id}")]
		public ActionResult GetById(int id)
		{
			return Ok(_repository.GetGiftById(id));
		}

	}
}
