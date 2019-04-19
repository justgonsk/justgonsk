﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JustGo.Helpers;
using JustGo.ServerConfigs;
using JustGo.View.Models;
using Microsoft.AspNetCore.Mvc;
using static JustGo.Helpers.Utilities;

namespace JustGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [StubExceptionFilter]
    public class TestController : ControllerBase
    {
        private HttpClient httpClient = HttpClientFactory.Create();

        [HttpGet]
        public async Task<Poll<EventViewModel>> Get()
        {
            var events = GetEventsFromTarget().Result;
            var query = Request.Query;

            if (query.ContainsKey(Constants.CategoriesKey))
            {
                var filter = new EventsFilter { FilterCategories = new List<string>() };
                var categories = query[Constants.CategoriesKey].ToString().Split(',');

                foreach (var category in categories)
                {
                    filter.FilterCategories.Add(category);
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
                result.Place = await GetPlaceById(result.Place.Id ?? -1);
                return result;
            }

            return null;
        }

        public async Task<Poll<EventViewModel>> GetEventsFromTarget()
        {
            #region old_impl

            /* var request = new HttpRequestMessage(HttpMethod.Get, Constants.EventPollUrl);

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Poll<EventViewModel>>();
            }
            else
            {
                return null;
            }  */

            #endregion old_impl

            var parsedPoll = await ParseResponseFromUrl(Constants.EventPollUrl);

            var pollInOurFormat = await ConvertToOurApiFormat(parsedPoll);

            var poll = pollInOurFormat.ToObject<Poll<EventViewModel>>(SnakeCaseSerializer);

            // kudago выдаёт неправильный count, поэтому пересчитываем сами
            poll.Count = poll.Results.Count;
            return poll;
        }
    }
}