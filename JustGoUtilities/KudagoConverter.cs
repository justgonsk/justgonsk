using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGoModels.Models;
using JustGoModels.Models.View;
using Newtonsoft.Json.Linq;
using static JustGoModels.Utilities;

namespace JustGoUtilities
{
    public static class KudagoConverter
    {
        public static async Task<PlaceViewModel> GetPlaceById(int placeId)
        {
            var body = await ParseResponseFromUrl(string.Format(Constants.PlaceDetailsUrlPattern, placeId));

            var place = body.ToObject<PlaceViewModel>();

            return place;
        }

        public static async Task<JObject> ConvertEventPoll(JObject kudagoPoll)
        {
            if (kudagoPoll == null)
                throw new ArgumentNullException(nameof(kudagoPoll));

            var ourPoll = new JObject(kudagoPoll);

            var results = (JArray)ourPoll["results"];

            for (int i = 0; i < results.Count; i++)
            {
                var convertedEvent = await ConvertEvent((JObject)results[i]);

                if (convertedEvent == null)
                {
                    results.RemoveAt(i);
                    i--;
                    continue;
                }

                results[i] = convertedEvent;
            }

            return ourPoll;
        }

        public static async Task<JObject> ConvertEvent(JObject kudagoEventInfo)
        {
            if (kudagoEventInfo == null)
                throw new ArgumentNullException(nameof(kudagoEventInfo));

            var eventInfo = new JObject(kudagoEventInfo);

            var placeProperty = eventInfo["place"];

            if (placeProperty.HasValues)
            {
                var placeId = (int)eventInfo["place"]["id"];
                var place = await GetPlaceById(placeId);
                eventInfo.Property("place").Value = JToken.FromObject(place, SnakeCaseSerializer);
            }
            else
            {
                return null;
            }

            var datesArray = (JArray)eventInfo["dates"];
            var singleDates = GetSingleDates(datesArray);
            var scheduledDates = GetScheduledDates(datesArray);

            eventInfo.Property("dates").Remove();
            eventInfo.Add("single_dates", JArray.FromObject(singleDates, SnakeCaseSerializer));
            eventInfo.Add("scheduled_dates", JArray.FromObject(scheduledDates, SnakeCaseSerializer));

            return eventInfo;
        }

        public static SingleDate[] GetSingleDates(JArray dates)
        {
            return dates.Where(entry => !HasSchedules(entry))
            .Select(entry =>
            {
                var startTimestamp = (long)entry["start"];
                var endTimestamp = (long)entry["end"];

                var start = DateTimeOffset
                    .FromUnixTimeSeconds(startTimestamp)
                    .ToLocalTime().DateTime;

                var end = DateTimeOffset
                    .FromUnixTimeSeconds(endTimestamp)
                    .ToLocalTime().DateTime;

                return new SingleDate(start, end);
            })
            .Where(date => date.Start.Year >= 2019 && date.End.Year <= 2021)
            .ToArray();
        }

        public static ScheduledDate[] GetScheduledDates(JArray dates)
        {
            return dates.Where(HasSchedules)
            .Select(entry =>
            {
                var startTimestamp = (long)entry["start"];
                var endTimestamp = (long)entry["end"];

                var scheduleStart = DateTimeOffset
                    .FromUnixTimeSeconds(startTimestamp)
                    .ToLocalTime().DateTime;

                var scheduleEnd = DateTimeOffset
                    .FromUnixTimeSeconds(endTimestamp)
                    .ToLocalTime().DateTime;

                var schedules = new List<Schedule>
                (
                    ((JArray)entry["schedules"])
                    .Select(schedule => schedule.ToObject<Schedule>(SnakeCaseSerializer))
                );

                return new ScheduledDate(scheduleStart, scheduleEnd, schedules);
            })
            .Where(date => date.ScheduleEnd.Year >= 2019)
            .ToArray();
        }

        private static bool HasSchedules(JToken entry)
        {
            var schedules = (JArray)entry["schedules"];

            return schedules != null && schedules.Count != 0;
        }
    }
}