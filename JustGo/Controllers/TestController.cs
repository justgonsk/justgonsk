using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JustGoUtilities.Exceptions;
using JustGo.ServerConfigs;
using JustGoModels.Models;
using JustGoModels.Models.View;
using JustGoUtilities;
using Microsoft.AspNetCore.Mvc;
using static JustGoUtilities.Utilities;

namespace JustGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [StubExceptionFilter]
    public class TestController : Controller
    {
        private HttpClient httpClient = HttpClientFactory.Create();

        [HttpGet]
        public async Task<Poll<EventViewModel>> Get()
        {
            var events = await GetEventsFromTarget();
            var query = Request.Query;

            if (query.ContainsKey(Constants.CategoriesKey))
            {
                var filter = new EventsFilter { RequiredCategories = new List<string>() };
                var categories = query[Constants.CategoriesKey].ToString().Split(',');

                foreach (var category in categories)
                {
                    filter.RequiredCategories.Add(category);
                }

                events = filter.FilterEvents(events.Results).ToPoll();
            }

            return events;
        }

        [HttpGet("{id}")]
        public async Task<EventViewModel> GetEvent(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.EventDetailsUrl + id);
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<EventViewModel>();
                result.Place = await GetPlaceById(result.Place.Id ?? -1, Constants.PlaceDetailsUrlPattern);
                return result;
            }

            return null;
        }

        public async Task<Poll<EventViewModel>> GetEventsFromTarget()
        {
            var parsedPoll = await ParseResponseFromUrl(Constants.EventPollUrl);

            var pollInOurFormat = await ConvertToOurApiFormat(parsedPoll, Constants.PlaceDetailsUrlPattern);

            var poll = pollInOurFormat.ToObject<Poll<EventViewModel>>(SnakeCaseSerializer);

            // kudago выдаёт неправильный count, поэтому пересчитываем сами
            poll.Count = poll.Results.Count;
            return poll;
        }
    }
}