using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JustGo.Interfaces;
using JustGo.ServerConfigs;
using JustGo.View.Models;
using static JustGo.Helpers.Utilities;

namespace JustGo.Helpers
{
    public class EventsPollDaemon
    {
        private string targetUrl;
        private HttpClient httpClient;
        private int timespan;
        IEventsRepository eventsRepository;
        IPlacesRepository placesRepository;

        public EventsPollDaemon(IEventsRepository eventsRepository, 
                                IPlacesRepository placesRepository, 
                                string targetUrl, 
                                int timespan)
        {
            this.targetUrl = targetUrl;
            this.timespan = timespan;
            this.httpClient = HttpClientFactory.Create();
            this.eventsRepository = eventsRepository;
            this.placesRepository = placesRepository;
        }

        public async void MainCycle()
        {
            while (true)
            {
                var eventsPoll = await GetEventsFromTarget();
                await PutEventsInDatabase(eventsPoll);
                Thread.Sleep(timespan);
            }
        }

        public async Task FillBaseOnce()
        {
            var events = await GetEventsFromTarget();

            events = events.Results.ToPoll();
             
            var tasks = events.Results.Select(async (x) =>
            {
                var place = await placesRepository.AddAsync(x.Place);
                x.Place.Id = place.Id;
                await eventsRepository.AddAsync(x);
            });

            await Task.WhenAll(tasks);
        }

        public async Task PutEventsInDatabase(Poll<EventViewModel> eventsPoll)
        {
            // занести события в нашу базу
            /*foreach(var @event in eventsPoll.Results)
            {
                var eventFromDatabase = await eventsRepository.FindAsync(@event.Id.Value);
                if (@event.Id != null && eventFromDatabase == null)
                {
                    await eventsRepository.AddAsync(@event);
                }
            }*/

            var tasks = eventsPoll.Results.Select(async (x) => await eventsRepository.AddAsync(x));
            await Task.WhenAll(tasks);
        }

        public async Task<Poll<EventViewModel>> GetEventsFromTarget()
        {
            var parsedPoll = await ParseResponseFromUrl(Constants.EventPollUrl);

            var pollInOurFormat = await ConvertToOurApiFormat(parsedPoll);

            var poll = pollInOurFormat.ToObject<Poll<EventViewModel>>(SnakeCaseSerializer);

            return poll;
        }
    }
}