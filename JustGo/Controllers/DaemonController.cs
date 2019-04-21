using System;
using System.Threading.Tasks;
using JustGo.Helpers;
using JustGo.Interfaces;
using JustGo.ServerConfigs;
using Microsoft.AspNetCore.Mvc;

namespace JustGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaemonController : Controller
    {
        private IEventsRepository eventsRepository { get; set; }
        private IPlacesRepository placesRepository { get; set; }
        //private EventsDaemon eventsDaemon;

        private bool eventsDaemonStarted = false;
        
        public DaemonController(IEventsRepository eventsRepository, IPlacesRepository placesRepository)
        {
            this.eventsRepository = eventsRepository;
            this.placesRepository = placesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> StartEventsDaemon()
        {
            if (!eventsDaemonStarted)
            {
                var eventsDaemon = new EventsDaemon(eventsRepository, placesRepository, Constants.EventPollUrl, Constants.EventsPollDaemonTimespan);
                eventsDaemonStarted = true;
                //Task.Factory.StartNew(() => eventsDaemon.MainCycle());
                await eventsDaemon.FillBaseOnce();
            }

            return Ok();
        }
    }
}
