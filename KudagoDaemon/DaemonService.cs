using System;
using System.Threading;
using System.Threading.Tasks;

//using JustGo.View.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

//using static JustGo.Helpers.Utilities;
using System.Linq;
using JustGoModels.Models.View;
using JustGoUtilities;
using static JustGoUtilities.Utilities;

namespace KudagoDaemon
{
    public class DaemonService : IHostedService, IDisposable
    {
        private const string DefaultEventPollUrl =
            "https://kudago.com/public-api/v1.4/events/?location=nsk&expand=dates&fields=id,dates,title,short_title,place,description,categories,images,tags&actual_since=1554508800";

        private const string DefaultEventDetailsUrl = "https://kudago.com/public-api/v1.4/events/";

        //32414 - максимальный ID места на кудаго на момент этого коммита
        private const string DefaultPlaceDetailsUrlPattern =
            "https://kudago.com/public-api/v1.4/places/{0}/?lang=&fields=id,title,address,coords&expand=";

        private int timespan;

        private readonly ILogger _logger;
        private readonly IOptions<DaemonConfig> _config;

        public DaemonService(ILogger<DaemonService> logger, IOptions<DaemonConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting daemon: " + _config.Value.DaemonName);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping daemon.");
            return Task.CompletedTask;
        }

        public async Task FillBaseOnce()
        {
            var events = await GetEventsFromTarget();

            events = events.Results.ToPoll();

            var tasks = events.Results.Select(async (x) =>
            {
                //var place = await AddPlaceToDatabase(IPlacesRepository, x.Place);
                //x.Place.Id = place.Id;
                //await eventsRepository.AddAsync(x);
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

            //var tasks = eventsPoll.Results.Select(async (x) => await eventsRepository.AddAsync(x));
            //await Task.WhenAll(tasks);
        }

        public async void MainCycle()
        {
            while (true)
            {
                await FillBaseOnce();
                Thread.Sleep(timespan);
            }
        }

        public async Task<Poll<EventViewModel>> GetEventsFromTarget()
        {
            var parsedPoll = await Utilities.ParseResponseFromUrl(DefaultEventPollUrl);

            var pollInOurFormat = await ConvertToOurApiFormat(parsedPoll, DefaultPlaceDetailsUrlPattern);

            var poll = pollInOurFormat.ToObject<Poll<EventViewModel>>(SnakeCaseSerializer);

            return poll;
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");
        }
    }
}