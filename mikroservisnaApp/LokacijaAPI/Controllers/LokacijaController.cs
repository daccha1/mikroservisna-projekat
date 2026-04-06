using Microsoft.AspNetCore.Mvc;
using mikroservisnaApp.Models.DTO.LokacijaDTO;
using ProductsAPI.Contracts;

namespace ProductsAPI.Controllers
{
	[ApiController]
	[Route("{controller}")]
	public class LokacijaController : Controller
	{
		private ILokacija _repository;
		public LokacijaController(ILokacija repository)
		{
			_repository = repository;
		}

		[HttpGet]
		public async Task<ActionResult<List<LokacijaResponseDTO>>> GetAll()
		{
			var result = await _repository.GetAll();
			if(result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<LokacijaResponseDTO>> GetById(int id)
		{
			var lokacija = await _repository.GetById(id);
			if(lokacija == null)
			{
				return NotFound();
			}
			return Ok(lokacija);
		}

	}
}
