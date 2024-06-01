using AutoMapper;
using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Models;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDbContext _context;
        private readonly IMapper _mapper;

        public DevEventsController(
            DevEventsDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        /// <summary>
        /// Obter todos os eventos
        /// </summary>
        /// <returns>Coleção de eventos</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var devEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();

            var ViewModel = _mapper.Map<List<DevEventViewModel>>(devEvents);

            return Ok(ViewModel);
        }

        /// <summary>
        /// Obter um evento
        /// </summary>
        /// <param name="id">Identificador do evento</param>
        /// <returns>Dados do evento</returns>
        /// <reponse code="200">Sucesso</reponse>
        /// <reponse code="404">Não encontrado</reponse>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _context.DevEvents
                .Include(de => de.Speakers)
                .SingleOrDefault(d => d.Id == id);
            if (devEvent == null)
            {
                return NotFound();
            }

            var ViewModel = _mapper.Map<DevEventViewModel>(devEvent);
            return Ok(ViewModel);
        }

        /// <summary>
        /// Cadastrar um evento
        /// </summary>
        /// <remarks>
        /// { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "title": "string", "description": "string", "startDate": "2024-05-28T00:44:31.752Z", "endDate": "2024-05-28T00:44:31.752Z", "speakers": [ { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "talkTitle": "string", "talkDescreption": "string", "linkedInProfile": "string", "devEventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" } ], "isDeleted": true }
        /// </remarks>
        /// <param name="input">Dados do evento</param>
        /// <returns>Objeto recém-criado</returns>
        /// <reponse code="201">Sucesso</reponse>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(DevEventInputModel input)
        {
            var devEvent = _mapper.Map<DevEvent>(input);
            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new {id = devEvent.Id}, devEvent);

        }
        /// <summary>
        /// Atualizar um evento
        /// </summary>
        /// <remarks>
        /// { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "title": "string", "description": "string", "startDate": "2024-05-28T00:44:31.752Z", "endDate": "2024-05-28T00:44:31.752Z", "speakers": [ { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "talkTitle": "string", "talkDescreption": "string", "linkedInProfile": "string", "devEventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" } ], "isDeleted": true }
        /// </remarks>
        /// <param name="id">Identificador do eveneto</param>
        /// <param name="input">Dados do evento</param>
        /// <returns>Nada.</returns>
        /// <reponse code="404">Não encontrado</reponse>
        /// <reponse code="204">Sucesso</reponse>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType (StatusCodes.Status204NoContent)]
        public IActionResult Update(Guid id, DevEventInputModel input)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }
  
            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deletar um evento
        /// </summary>
        /// <param name="id">Identificador do evento</param>
        /// <returns>Nada.</returns>
        /// <reponse code="404">Não encontrado</reponse>
        /// <reponse code="204">Sucesso</reponse>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);
            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Delete();

            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Cadastrar palestrante 
        /// </summary>
        /// <remarks>
        /// { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "talkTitle": "string", "talkDescreption": "string", "linkedInProfile": "string", "devEventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" }
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="input">Dados do palestrante</param>
        /// <returns>Nada.</returns>
        /// <reponse code="404">Não encontrado</reponse>
        /// <reponse code="204">Sucesso</reponse>
        [HttpPost("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PostSpeakers(Guid id, DevEventSpeakerInputModel input)
        {
            var speaker = _mapper.Map<DevEventSpeaker>(input);

            speaker.DevEventId = id;

            var devEvent = _context.DevEvents.Any(d => d.Id == id);
            if (!devEvent)
            {
                return NotFound();
            }

            _context.DevEventSpeakers.Add(speaker);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
