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
    [StubExceptionFilter]
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

        #region EXAMPLE

        /* POST: api/Events
    json:
{
        "dates": [
                    {
                        "start": "2016-07-01",
                        "end": "2016-10-02"
                    },
                    {
                        "start": "2019-02-12",
                        "end": "2019-05-12"
                    }

            ],
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
                },
                {
                    "image": "https://kudago.com/media/images/event/e4/fd/e4fda345d65a1a5a4cd808240fe25b4f.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                },
                {
                    "image": "https://kudago.com/media/images/event/e2/50/e250df4d047c74101c72beba7c4f0dca.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                },
                {
                    "image": "https://kudago.com/media/images/event/56/7b/567b7731c1e3284e1cda9a08af51bb56.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                },
                {
                    "image": "https://kudago.com/media/images/event/a4/e3/a4e3be113b5ae5879b3b59fefc679e32.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                },
                {
                    "image": "https://kudago.com/media/images/event/fd/5a/fd5a91b31e61e31bdf8aeaf889b99ce5.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                },
                {
                    "image": "https://kudago.com/media/images/event/1a/89/1a89705da915398572538d7fc4a736fc.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                },
                {
                    "image": "https://kudago.com/media/images/event/4d/a8/4da896541296628efeb1cb459393c95f.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                },
                {
                    "image": "https://kudago.com/media/images/event/60/84/6084adda0fca1abb0cb3884c88e35f41.jpg",
                    "source": {
                        "name": "",
                        "link": ""
                    }
                }
            ],
            "short_title": "Шедевры импрессионизма. Том 1. Винсент Ван Гог и Эдуард Мане",
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
            ]
}
*/

        #endregion EXAMPLE

        [HttpPost]
        public async Task<ActionResult<EventViewModel>> AddEventAsync([FromBody] EventViewModel eventViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await eventsRepository.AddAsync(eventViewModel);

            return CreatedAtAction(actionName: nameof(GetEventAsync),
                routeValues: new { id = @event.Id }, value: @event.ToViewModel());
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