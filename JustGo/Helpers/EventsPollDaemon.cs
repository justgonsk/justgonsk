using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JustGo.View.Models;

namespace JustGo.Helpers
{
    public class EventsPollDaemon
    {
        private string targetUrl;
        private HttpClient httpClient;
        private int timespan;

        public EventsPollDaemon(string targetUrl, int timespan)
        {
            this.targetUrl = targetUrl;
            this.timespan = timespan;
            this.httpClient = HttpClientFactory.Create();
            var events = GetEvents().Result;

            foreach (var e in events)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<IEnumerable<Event>> GetEvents()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, targetUrl);
            //request.Headers.Add("Accept", "application/vnd.github.v3+json");
            //request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<Event>>();
            }
            else
            {
                return null;
            }
        }
    }
}
