using System.Threading.Tasks;
using JustGo.Exceptions;
using JustGo.Helpers;
using JustGo.Interfaces;
using JustGo.View.Models;
using JustGo.View.Models.Edit;
using Microsoft.AspNetCore.Mvc;

namespace JustGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [StubExceptionFilter]
    public class PlacesController : Controller
    {
        private readonly IPlacesRepository placesRepository;

        public PlacesController(IPlacesRepository placesRepository)
        {
            this.placesRepository = placesRepository;
        }

        // GET: api/Places/?offset=2&count=35
        [HttpGet]
        public Poll<PlaceViewModel> GetPlacePoll([FromQuery] int? offset, [FromQuery] int? count)
        {
            return placesRepository.GetPlacePoll(offset, count);
        }

        // GET: api/Places/2903
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaceAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var place = await placesRepository.FindAsync(id);

            if (place == null)
            {
                return NotFound();
            }

            return Ok(place.ToViewModel());
        }

        /* POST: api/Places

        json:
        {
            "title": "шоколадная лавка Marie Chérie",
            "address": "ул. Гороховая, д. 55.",
            "coords": {
                "lat": 59.9264890000002,
                "lon": 30.32464199999989
                }
        }
}
        */

        [HttpPost]
        public async Task<ActionResult<PlaceViewModel>> AddPlaceAsync([FromBody] PlaceViewModel eventViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var place = await placesRepository.AddAsync(eventViewModel);

            return CreatedAtAction(actionName: nameof(GetPlaceAsync),
                routeValues: new { id = place.Id }, value: place.ToViewModel());
        }

        /* PUT: api/Places/32

        json:
        "title": "шоколадная лавка Marie Chérie",
        "address": "ул. Гороховая, д. 55.",
        "coords": {
            "lat": 59.9264890000002,
            "lon": 30.32464199999989
            }
        ]
        */

        [HttpPut("{id}")]
        public async Task<ActionResult<PlaceViewModel>> UpdatePlaceAsync([FromRoute] int id,
            [FromBody] PlaceEditModel eventViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!PlaceExists(id))
            {
                return NotFound();
            }

            var place = await placesRepository.UpdateAsync(id, eventViewModel);

            return place.ToViewModel();
        }

        // DELETE: api/Places/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaceAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!PlaceExists(id))
            {
                return NotFound();
            }

            var place = await placesRepository.DeleteAsync(id);

            return Ok(place.ToViewModel());
        }

        private bool PlaceExists(int id)
        {
            return placesRepository.FindAsync(id).Result != null;
        }
    }
}