using System;
using System.Net.Http;
using System.Threading.Tasks;
using JustGo.Extern.Models;
using JustGo.Helpers;
using JustGo.Models;
using JustGo.ServerConfigs;
using JustGo.View.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private HttpClient httpClient = HttpClientFactory.Create();

        [HttpGet]
        public async Task<Poll<KudagoEvent>> Get()
        {
            var events = GetEventsFromTarget().Result;
            var query = Request.Query;

            if (query.ContainsKey(Constants.CategoriesKey))
            {
                var filter = new EventsFilter();
                var categories = query[Constants.CategoriesKey].ToString().Split(',');

                foreach (var category in categories)
                {
                    filter.Categories.Add(category);
                }
            }

            foreach (var e in events.Results)
            {
                e.Place = await Utilities.GetPlaceById(e.Place.Id);
            }

            return events;
        }

        [HttpGet("{id}")]
        public async Task<KudagoEvent> GetEvent(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.EventDetailsUrl + id);
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<KudagoEvent>();
                result.Place = await Utilities.GetPlaceById(result.Place.Id);
                return result;
            }
            else
            {
                return null;
            }
        }

        public async Task<Poll<KudagoEvent>> GetEventsFromTarget()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.EventPollUrl);

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Poll<KudagoEvent>>();
            }
            else
            {
                return null;
            }
        }
    }
}