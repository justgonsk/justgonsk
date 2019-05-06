using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Linq;
using JustGoModels.Models.View;
using JustGoUtilities;
using static JustGoModels.Utilities;
using static KudagoDaemon.PlaceManager;
using Microsoft.EntityFrameworkCore;
using JustGoUtilities.Data;
using JustGoUtilities.Repositories;

namespace KudagoDaemon
{
    public class DaemonService : IHostedService, IDisposable
    {
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

        private async Task FillBaseOnce()
        {
            var events = await GetAllEventsFromTarget();

            await PutEventsInDatabase(events);
        }

        private async Task PutEventsInDatabase(Poll<EventViewModel> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var allEvents = eventsRepository.EnumerateAll();

            var tasks = events.Results.Select(async (x) =>
            {
                if (!allEvents.Any(eventFromDb => x.Description == eventFromDb.Description))
                {
                    if (x.SingleDates.Count == 0 && x.ScheduledDates.Count == 0)
                    {
                        return;
                    }

                    var place = await AddPlaceToDatabase(placesRepository, x.Place);
                    x.Place.Id = place.Id;

                    await eventsRepository.AddAsync(x);
                }
            });

            await Task.WhenAll(tasks);
        }

        private async Task MainCycle()
        {
            while (true)
            {
                _config.Value.ReadConfigFromFile("Config.xml");

                await FillBaseOnce();
                Thread.Sleep(_config.Value.Timespan);
            }
        }

        private async Task<Poll<EventViewModel>> GetAllEventsFromTarget()
        {
            var nowUnixTimestamp = DateTime.UtcNow.ToUnixTimeSeconds();
            var endRangeTimestamp = DateTime.UtcNow.AddDays(_config.Value.DateTimeRangeLengthInDays).ToUnixTimeSeconds();

            var events = await GetEventsFromTarget(string.Format(_config.Value.EventPollUrlPattern, nowUnixTimestamp, endRangeTimestamp));
            var result = new Poll<EventViewModel>(events.Results);

            while (events.Next != null)
            {
                events = await GetEventsFromTarget(events.Next);
                var eventsPoll = new Poll<EventViewModel>(events.Results);
                result.AddRange(eventsPoll);
            }

            return result;
        }

        private async Task<Poll<EventViewModel>> GetEventsFromTarget(string targetURL)
        {
            var parsedPoll = await ParseResponseFromUrl(targetURL);

            var pollInOurFormat = await KudagoConverter.ConvertEventPoll(parsedPoll);

            var poll = pollInOurFormat.ToObject<Poll<EventViewModel>>(SnakeCaseSerializer);

            return poll;
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");
        }
    }
}