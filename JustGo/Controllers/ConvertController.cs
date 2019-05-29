﻿using System.Linq;
using System.Threading.Tasks;
using JustGoModels;
using JustGoModels.Models;
using JustGoModels.Models.View;
using JustGoUtilities;
using JustGoUtilities.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace JustGo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [StubExceptionFilter]
    public class ConvertController : Controller
    {
        [HttpGet("Event/{kudagoEventId}")]
        public async Task<JsonResult> ConvertEvent(int kudagoEventId)
        {
            var parsedEvent = await Utilities.ParseResponseFromUrl(string
                .Format(Constants.EventDetailsUrlPattern, kudagoEventId));

            var serialized = await KudagoConverter.ConvertEvent(parsedEvent);

            if (serialized == null)
            {
                return Json("Place of this event is not set on Kudago");
            }

            var viewModel = serialized.ToObject<EventViewModel>(Utilities.SnakeCaseSerializer);

            var model = new Event
            {
                SingleDates = viewModel.SingleDates.ToList(),
                ScheduledDates = viewModel.ScheduledDates.ToList()
            };

            viewModel.Current = model.FindCurrent();
            var novosibirskNow = Utilities.NovosibirskNow;

            viewModel.NextOnWeek = model
                .FindFirstInRange(novosibirskNow, novosibirskNow.AddDays(7));

            return Json(viewModel);
        }
    }
}