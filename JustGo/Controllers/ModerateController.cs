using System.Linq;
using System.Threading.Tasks;
using JustGo.Policies;
using JustGoModels.Interfaces;
using JustGoModels.Models;
using JustGoModels.Models.Edit;
using JustGoModels.Models.View;
using JustGoUtilities;
using JustGoUtilities.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace JustGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [StubExceptionFilter]
    [Authorize(Roles = nameof(Admins))]
    public class ModerateController : Controller
    {
        private readonly IEventsRepository eventsRepository;

        public ModerateController(IEventsRepository eventsRepository)
        {
            this.eventsRepository = eventsRepository;
        }

        [HttpGet]
        public ActionResult<Poll<EventViewModel>> GetModerationPoll()
        {
            return eventsRepository.EnumerateAll()
                .Where(e => !e.IsModerated).ToPoll<Event, EventViewModel>();
        }

        // PUT: api/Moderate/Accept/2903
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Accept([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await eventsRepository.UpdateAsync(id, new EventEditModel { IsModerated = true });

            if (@event == null)
            {
                return NotFound();
            }

            return Ok(@event.ToViewModel());
        }

        // PUT: api/Moderate/Decline/2903
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Decline([FromRoute] int id)
        {
            return await new EventsController(eventsRepository).DeleteEventAsync(id);
        }
    }
}