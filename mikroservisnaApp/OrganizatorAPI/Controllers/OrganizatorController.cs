using Microsoft.AspNetCore.Mvc;
using mikroservisnaApp.Models.DTO.OrganizatorDTO;
using OrganizatorAPI.Contracts;

namespace OrganizatorAPI.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class OrganizatorController : ControllerBase
    {
        private IOrganizator _repository;

        public OrganizatorController(IOrganizator repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrganizatorResponseDTO>>> GetAll()
        {
            var result = await _repository.GetAll();
            if (result == null)
            {
                return NotFound();
            }
			await Task.Delay(7000); //-> testiranje client timeouta na glavnom projektu
			return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizatorResponseDTO>> GetById(int id)
        {
            var organizator = await _repository.GetById(id);
            if (organizator == null)
            {
                return NotFound("Ne postoji dati organizator.");
            }
            return Ok(organizator);
        }

        [HttpPost("add")]
        public async Task<ActionResult<OrganizatorRequestDTO>> Post([FromBody] OrganizatorRequestDTO organizatorToAdd)
        {
            var isAdded = await _repository.Post(organizatorToAdd);
            if (isAdded == null)
            {
                return BadRequest("Nije moguce dodati dati objekat.");
            }
            return Ok(isAdded);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrganizatorRequestDTO organizatorToUpdate)
        {
            var successful = await _repository.Update(id, organizatorToUpdate);
            if (!successful)
            {
                return BadRequest("Nije moguce izvrsiti update.");
            }
            return Ok();
        }
    }
}
