using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JustGo.Extern.Models;
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
        }

        public async void MainCycle()
        {
            while (true)
            {
                var eventsPoll = await GetEventsFromTarget();
                PutEventsInDatabase(eventsPoll);
                Thread.Sleep(timespan);
            }
        }

        public void PutEventsInDatabase(Poll<KudagoEvent> eventsPoll)
        {
            // занести события в нашу базу
            throw new NotImplementedException();
        }

        public async Task<Poll<KudagoEvent>> GetEventsFromTarget()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, targetUrl);
            //request.Headers.Add("Accept", "application/vnd.github.v3+json");
            //request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

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