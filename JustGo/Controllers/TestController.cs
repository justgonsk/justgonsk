using System;
using System.Net.Http;
using System.Threading.Tasks;
using JustGo.Helpers;
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
        [HttpGet]
        public async Task<EventsPoll> Get()
        {
            var events = GetEventsFromTarget().Result;

            foreach (var e in events.Results)
            {
                e.Place = await Utilities.GetPlaceById(e.Place.Id);
            }

            return events;
        }

        public async Task<EventsPoll> GetEventsFromTarget()
        {
            var httpClient = HttpClientFactory.Create();
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.EventPollUrl);

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<EventsPoll>();
            }
            else
            {
                return null;
            }
        }
    }
}
