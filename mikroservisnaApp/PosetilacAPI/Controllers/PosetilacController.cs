using Microsoft.AspNetCore.Mvc;
using mikroservisnaApp.Models.DTO.PosetilacDTO;
using PosetilacAPI.Contracts;

namespace PosetilacAPI.Controllers
{
    [ApiController]
    [Route("{posetioci}")]
    public class PosetilacController : ControllerBase
    {
        private IPosetilac _repository;

        public PosetilacController(IPosetilac repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<PosetilacResponseDTO>>> GetAll()
        {
            var result = await _repository.GetAll();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PosetilacResponseDTO>> GetById(int id)
        {
            var posetilac = await _repository.GetById(id);
            if (posetilac == null)
            {
                return NotFound("Ne postoji dati posetilac.");
            }
            return Ok(posetilac);
        }

        [HttpPost("add")]
        public async Task<ActionResult<PosetilacRequestDTO>> Post([FromBody] PosetilacRequestDTO posetilacToAdd)
        {
            var isAdded = await _repository.Post(posetilacToAdd);
            if (isAdded == null)
            {
                return BadRequest("Nije moguce dodati dati objekat.");
            }
            return Ok(isAdded);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PosetilacRequestDTO posetilacToUpdate)
        {
            var successful = await _repository.Update(id, posetilacToUpdate);
            if (!successful)
            {
                return BadRequest("Nije moguce izvrsiti update.");
            }
            return Ok();
        }
    }
}
