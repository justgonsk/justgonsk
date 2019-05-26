﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JustGoUtilities.Exceptions;
using JustGoModels.Models;
using JustGoModels.Models.View;
using JustGoUtilities;
using Microsoft.AspNetCore.Mvc;
using static JustGoModels.Utilities;

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
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(Constants.EventDetailsUrlPattern, id));
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<EventViewModel>();
                result.Place = await KudagoConverter.GetPlaceById(result.Place.Id ?? -1);
                return result;
            }

            return null;
        }

        public async Task<Poll<EventViewModel>> GetEventsFromTarget()
        {
            var parsedPoll = await ParseResponseFromUrl(Constants.EventPollUrl);

            var pollInOurFormat = await KudagoConverter.ConvertEventPoll(parsedPoll);

            var poll = pollInOurFormat.ToObject<Poll<EventViewModel>>(SnakeCaseSerializer);

            return poll;
        }
    }
}