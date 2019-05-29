using System.Threading.Tasks;
using JustGoUtilities.Exceptions;
using JustGoModels.Interfaces;
using JustGoModels.Models.Edit;
using JustGoModels.Models.View;
using JustGoModels.Policies;
using JustGoUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JustGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [StubExceptionFilter]
    public class EventsController : Controller
    {
        private readonly IEventsRepository eventsRepository;

        public EventsController(IEventsRepository eventsRepository)
        {
            this.eventsRepository = eventsRepository;
        }

        #region EXAMPLE

        /* GET: api/Events/filter/?offset=2&count=35

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

        #endregion EXAMPLE

        [HttpGet]
        public ActionResult<Poll<EventViewModel>> GetEventPoll([FromQuery] int? offset,
            [FromQuery] int? count, [FromQuery] EventsFilterViewModel filterInfo)
        {
            var filter = filterInfo.ToModel();
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

            return Ok(@event.ToViewModel());
        }

        #region EXAMPLE

        /* POST: api/Events
    json:
{
            "title": "выставка «Шедевры импрессионизма. Том 1. Винсент Ван Гог и Эдуард Мане»",
            "place": {
                "id": 32250
            },
            "description": "<p>Галерея  «Мольбеrt» принимает в своих стенах уникальную мультимедийную выставку, где будет представлено более 150 работ двух великих импрессионистов.</p>\n",
            "categories": [
                "exhibition",
                "show"
            ],
            "images": [
                {
                    "image": "https://kudago.com/media/images/event/67/2d/672d98ba53bdeb6ae49becb587708c4b.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                },
                {
                    "image": "https://kudago.com/media/images/event/3a/26/3a2698aef58d77d0cd1af611954bc0cc.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                },
                {
                    "image": "https://kudago.com/media/images/event/3f/2f/3f2f0dffcd1b42d64d0a9d1e72469bdb.JPG",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                }
            ],
            "short_title": "Шедевры импрессионизма. Том 1. Винсент Ван Гог и Эдуард Мане",
            "source": "Kudago.com",
            "tags": [
                "шоу (развлечения)",
                "мультимедиа",
                "культура и искусство",
                "красиво",
                "новое на сайте",
                "интересное",
                "картины, живопись, графика",
                "выставки",
                "12+"
            ],
            "single_dates": [
            {
                "start": "2019-06-03T09:00:00",
                "end": "2019-06-14T14:30:00"
            },
            {
                "start": "2019-06-17T09:00:00",
                "end": "2019-06-28T14:30:00"
            },
            {
                "start": "2019-07-01T09:00:00",
                "end": "2019-07-12T14:30:00"
            },
            {
                "start": "2019-08-01T09:00:00",
                "end": "2019-08-14T14:30:00"
            }
        ],

        "scheduled_dates": [
            {
                "schedule_start": "0001-01-03T03:30:00",
                "schedule_end": "9999-01-01T04:00:00",
                "schedules": [
                    {
                        "days_of_week": [
                            1,
                            2,
                            3,
                            5
                        ],
                        "start_time": "16:00:00",
                        "end_time": "17:30:00"
                    }
                ]
            }
        ]
}
*/

        #endregion EXAMPLE

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EventViewModel>> AddEventAsync([FromBody] EventViewModel eventViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            eventViewModel.Source = User.Identity.Name;
            eventViewModel.Moderated = this.IsAdmin();

            var @event = await eventsRepository.AddAsync(eventViewModel);

            return CreatedAtAction(actionName: nameof(GetEventAsync),
                routeValues: new { id = @event.Id }, value: @event.ToViewModel());
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        [Authorize(Roles = nameof(Users))]
        public async Task<ActionResult<EventViewModel>> UpdateEventAsync([FromRoute] int id,
            [FromBody] EventEditModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!EventExists(id))
            {
                return NotFound();
            }

            var @event = await eventsRepository.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            if (@event.Source != User.Identity.Name && !this.IsAdmin())
            {
                return Forbid();
            }

            @event = await eventsRepository.UpdateAsync(id, editViewModel);

            return @event.ToViewModel();
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(Admins))]
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

            return Ok(@event.ToViewModel());
        }

        private bool EventExists(int id)
        {
            return eventsRepository.FindAsync(id).Result != null;
        }
    }
}