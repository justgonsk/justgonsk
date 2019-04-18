using System.Threading.Tasks;
using JustGo.Helpers;
using JustGo.Interfaces;
using JustGo.View.Models;
using Microsoft.AspNetCore.Mvc;

namespace JustGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

            return Ok(place);
        }

        // POST: api/Places
        [HttpPost]
        public async Task<ActionResult<PlaceViewModel>> AddPlaceAsync([FromBody] PlaceViewModel eventViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var place = await placesRepository.AddAsync(eventViewModel);

            return CreatedAtAction(actionName: nameof(GetPlaceAsync),
                routeValues: new { id = place.Id }, value: eventViewModel);
        }

        // PUT: api/Places/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PlaceViewModel>> UpdatePlaceAsync([FromRoute] int id, [FromBody] PlaceViewModel eventViewModel)
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

            return Ok(place);
        }

        private bool PlaceExists(int id)
        {
            return placesRepository.FindAsync(id).Result != null;
        }
    }
}