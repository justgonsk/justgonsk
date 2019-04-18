using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JustGo.Data;
using JustGo.Helpers;
using JustGo.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JustGo.Models;
using JustGo.View.Models;

namespace JustGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : Controller
    {
        private readonly IEventsRepository eventsRepository;

        public EventsController(IEventsRepository eventsRepository)
        {
            this.eventsRepository = eventsRepository;
        }

        /* GET: api/Events/?offset=2&count=35

        json:

         "tags": [
            "шоу (развлечения)",
            "мультимедиа",
            "культура и искусство",
            "красиво"
        ],

          "categories": [
                "exhibition",
                "show"
        ],

           "places": [
                12705,
                302,
                24
        ]

             */

        [HttpGet]
        public Poll<EventViewModel> GetEventPoll([FromBody] EventsFilter filter,
            [FromQuery] int? offset, [FromQuery] int? count)
        {
            return eventsRepository.GetEventPoll(filter, offset, count);
        }

        // GET: api/Events/2903
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await eventsRepository.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            return Ok(@event);
        }

        // POST: api/Events
        [HttpPost]
        public async Task<ActionResult<EventViewModel>> AddEventAsync([FromBody] EventViewModel eventViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await eventsRepository.AddAsync(eventViewModel);

            return CreatedAtAction(actionName: nameof(GetEventAsync),
                routeValues: new { id = @event.Id }, value: eventViewModel);
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        public async Task<ActionResult<EventViewModel>> UpdateEventAsync([FromRoute] int id, [FromBody] EventViewModel eventViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!EventExists(id))
            {
                return NotFound();
            }

            var @event = await eventsRepository.UpdateAsync(id, eventViewModel);

            return @event.ToViewModel();
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!EventExists(id))
            {
                return NotFound();
            }

            var @event = await eventsRepository.DeleteAsync(id);

            return Ok(@event);
        }

        private bool EventExists(int id)
        {
            return eventsRepository.FindAsync(id).Result != null;
        }
    }
}