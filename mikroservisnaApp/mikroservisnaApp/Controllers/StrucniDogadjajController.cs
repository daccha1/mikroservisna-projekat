using Common.EventService;
using Common.StrucniDogadjajDTO;
using ContractsCQRS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using mikroservisnaApp.Contracts;
using mikroservisnaApp.CQRS_Container.Application.Commands;
using mikroservisnaApp.CQRS_Container.Application.Queries;
using mikroservisnaApp.Models;
using mikroservisnaApp.Models.DTO;
using mikroservisnaApp.Models.DTO.StrucniDogadjajDTO;
using mikroservisnaApp.MQ_Container;
using mikroservisnaApp.Repositories.SQL_Server;
using StrucniDogadjaj.Domain.Write;
using StrucniDogadjaj.Infrastructure.Read.EFCore.Data;
using StrucniDogadjaj.Infrastructure.Write.EFCore.Data;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading.Tasks;

namespace mikroservisnaApp.Controllers
{
	[ApiController]
	[Route("events")]
	public class StrucniDogadjajController : ControllerBase
	{
		private IStrucniDogadjaj _repository;
		private IMQClient _mqPublisher;
		private WriteDbContext _write;
		private AddDogadjajCommandHandler _addDogadjajHandler;
		private EditDogadjajCommandHandler _editDogadjajHandler;
		private ReadDbContext _read;
		private DeleteDogadjajCommandHandler _deleteHandler;
		private GetAllDogadjajiQueryHandler _getAllHandler;
		private GetDetailsDogadjajQueryHandler _detailsHandler;
		private FilterByCenaDogadjajQueryHandler _cenaHandler;
		public StrucniDogadjajController(IStrucniDogadjaj repository, IMQClient mqPublisher, WriteDbContext write, ReadDbContext read, AddDogadjajCommandHandler handler, EditDogadjajCommandHandler editHandler, DeleteDogadjajCommandHandler deleteHandler, GetAllDogadjajiQueryHandler getAllHandler, GetDetailsDogadjajQueryHandler detailsHandler, FilterByCenaDogadjajQueryHandler cenaFilterHandler)
		{
			_mqPublisher = mqPublisher;
			_repository = repository;
			_write = write;
			_addDogadjajHandler = handler;
			_read = read;
			_editDogadjajHandler = editHandler;
			_deleteHandler = deleteHandler;
			_getAllHandler = getAllHandler;
			_cenaHandler = cenaFilterHandler;
			_detailsHandler = detailsHandler;
		}

		[HttpGet("")]
		public async Task<IActionResult> GetAll()
		{
			var result = await _getAllHandler.Handle(new GetAllDogadjajiQuery());
			if (result != null) return Ok(result);
			return NoContent();

			//var events = await _repository.GetAll();
			//if (events.Count == 0)
			//{
			//	return NotFound("Trenutno nema dogadjaja.");
			//}
			//return Ok(events);
		}


		[HttpGet("{id}")]
	
		public async Task<IActionResult> GetById(int id)
		{
			GetDetailsDogadjajQuery query = new() 
			{ 
				IdDogadjaja = id
			};
			var result = await _detailsHandler.Handle(query);
			if (result != null) return Ok(result);
			return NoContent();
		}

		[HttpGet("/price-filter")]
		public async Task<IActionResult> GetByCenaFilter([FromQuery] FilterByCenaDogadjajQuery query)
		{
			var result = await _cenaHandler.Handle(query);
			if (result != null) return Ok(result);
			return NoContent();
		}


		[HttpPost("add")]
		public async Task<IActionResult> Post([FromBody] AddDogadjajCommand dogadjaj)
		{

			int idDogadjaja = await _addDogadjajHandler.Handle(dogadjaj);

			return Ok(idDogadjaja);

			//var newEvent = await _repository.Post(addEvent);
			//if(newEvent == null)
			//{
			//	return NotFound("Nije uspelo dodavanje dogadjaja.");
			//}
			////string jsonBody = JsonSerializer.Serialize<StrucniDogadjajRequestDTO>(addEvent);
			////await _mqPublisher.SendMessageAsync(jsonBody, "events.event.eventsExchange", "event-publish-key");
			//return Ok(newEvent);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] EditDogadjajCommand dogadjaj)
		{
			dogadjaj.Id = id;
			int result = await _editDogadjajHandler.Handle(dogadjaj);
			return Ok(result);
		}

		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			DeleteDogadjajCommand cmd = new()
			{
				IdDogadjaja = id
			};
			int result = await _deleteHandler.Handle(cmd);

			if (result == 0) return NotFound(result);
			return Ok(result);
		}

	}
}
