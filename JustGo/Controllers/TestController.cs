﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
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
        private const string TargetUrl = "https://kudago.com/public-api/v1.4/events/?" +
        	"location=nsk&expand=dates&fields=dates,title,short_title,place,description,categories," +
        	"images&actual_since=1554508800";

        [HttpGet]
        public ActionResult<EventsPoll> Get()
        {
            var events = GetEventsFromTarget();
            return events.Result;
        }

        public async Task<EventsPoll> GetEventsFromTarget()
        {
            var httpClient = HttpClientFactory.Create();
            var request = new HttpRequestMessage(HttpMethod.Get, TargetUrl);

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
