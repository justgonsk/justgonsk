using System.Threading.Tasks;
using JustGoModels.Models.View;
using JustGoUtilities;
using JustGoUtilities.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JustGo
{
    [ApiController]
    [Route("api/[controller]")]
    [StubExceptionFilter]
    public class ConvertController : Controller
    {
        [HttpGet("Event/{kudagoEventId}")]
        public async Task<EventViewModel> ConvertEvent(int kudagoEventId)
        {
            var parsedEvent = await Utilities.ParseResponseFromUrl(string
                .Format(Constants.EventDetailsUrlPattern, kudagoEventId));

            var serialized = await KudagoConverter.ConvertEvent(parsedEvent);

            return serialized.ToObject<EventViewModel>(Utilities.SnakeCaseSerializer);
        }
    }
}