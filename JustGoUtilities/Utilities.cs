using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JustGo.Models;
using JustGo.View.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JustGo.Helpers
{
    public static class Utilities
    {
        public static Dictionary<long, PlaceViewModel> PlacesInfoCache { get; }
            = new Dictionary<long, PlaceViewModel>();

        public static JsonSerializerSettings SnakeCaseSettings { get; }
            = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

        public static JsonSerializer SnakeCaseSerializer { get; } = JsonSerializer.Create(SnakeCaseSettings);

        public static async Task<JObject> ParseResponseFromUrl(string url)
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JObject.Parse(content);
        }

        public static async Task<JObject> ConvertToOurApiFormat(JObject parsedBody, string placeDetailsUrlPattern)
        {
            if (parsedBody == null)
                throw new ArgumentNullException(nameof(parsedBody));

            var newBody = new JObject(parsedBody);

            var results = (JArray)newBody["results"];

            foreach (var result in results)
            {
                var eventInfo = (JObject)result;

                var placeId = (int)eventInfo["place"]["id"];

                var place = await GetPlaceById(placeId, placeDetailsUrlPattern);

                eventInfo.Property("place").Value = JToken.FromObject(place, SnakeCaseSerializer);

                var dates = GetDatesFromJson((JArray)eventInfo["dates"]);

                eventInfo.Property("dates").Remove();
                eventInfo.Add("dates", new JArray());

                foreach (var date in dates)
                {
                    var (start, end) = date;
                    ((JArray)eventInfo["dates"]).Add(new JObject
                    (
                        new JProperty("start", start),
                        new JProperty("end", end)
                    ));
                }
            }

            return newBody;
        }

        private static (DateTime, DateTime)[] GetDatesFromJson(JArray dates)
        {
            return dates.Select(entry =>
            {
                var startTimestamp = (long)entry["start"];
                var endTimestamp = (long)entry["end"];

                var start = DateTimeOffset
                    .FromUnixTimeSeconds(startTimestamp)
                    .ToLocalTime().DateTime;

                var end = DateTimeOffset
                    .FromUnixTimeSeconds(endTimestamp)
                    .ToLocalTime().DateTime;

                return (start, end);
            }).ToArray();
        }

        public static async Task<PlaceViewModel> GetPlaceById(int placeId, string placeDetailsUrlPattern)
        {
            if (!PlacesInfoCache.ContainsKey(placeId))
            {
                var body = await ParseResponseFromUrl(string.Format(placeDetailsUrlPattern, placeId));

                var place = body.ToObject<PlaceViewModel>();

                PlacesInfoCache[placeId] = place;

                return place;
            }
            else
            {
                return PlacesInfoCache[placeId];
            }
        }
    }
}