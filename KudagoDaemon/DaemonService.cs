using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Linq;
using JustGoModels.Models.View;
using JustGoUtilities;
using static JustGoUtilities.Utilities;
using static KudagoDaemon.PlaceManager;
using Microsoft.EntityFrameworkCore;
using JustGoUtilities.Data;
using JustGoUtilities.Repositories;

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
        private MainContext _dbcontext;
        private DbEventsRepository eventsRepository;
        private DbPlacesRepository placesRepository;

        private readonly ILogger _logger;
        private readonly IOptions<DaemonConfig> _config;

        public DaemonService(ILogger<DaemonService> logger, IOptions<DaemonConfig> config)
        {
            _logger = logger;
            _config = config;

            var dbcontextoptions = new DbContextOptions<MainContext>();
            var optionsBuilder = new DbContextOptionsBuilder<MainContext>();
            optionsBuilder.UseMySQL("Server = localhost; Database = JustGo; User = root; Password = password;");

            _dbcontext = new MainContext(optionsBuilder.Options);
            eventsRepository = new DbEventsRepository(_dbcontext);
            placesRepository = new DbPlacesRepository(_dbcontext);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting daemon: " + _config.Value.DaemonName);
            MainCycle();
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
                if (!EventExistsInDb(x))
                {
                    var place = await AddPlaceToDatabase(placesRepository, x.Place);
                    x.Place.Id = place.Id;

                    await eventsRepository.AddAsync(x);
                }
            });

            await Task.WhenAll(tasks);
        }

        private bool EventExistsInDb(EventViewModel @event)
        {
            var poll = eventsRepository.GetEventPoll(null, 0, 1000000);
            return poll.Results.Any(x => x.Description == @event.Description);
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
            var parsedPoll = await ParseResponseFromUrl(DefaultEventPollUrl);

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