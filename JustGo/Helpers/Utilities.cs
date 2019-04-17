using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JustGo.Extern.Models;
using JustGo.Models;
using JustGo.ServerConfigs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JustGo.Helpers
{
    public static class Utilities
    {
        public static Dictionary<long, KudagoPlace> PlacesInfoCache { get; } = new Dictionary<long, KudagoPlace>();

        public static JsonSerializerSettings SnakeCaseSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public static JsonSerializer SnakeCaseSerializer { get; } = JsonSerializer.Create(SnakeCaseSettings);

        public static void Rename(this JToken token, string newName)
        {
            var parent = token.Parent;
            var newToken = new JProperty(newName, token);
            parent.Replace(newToken);
        }

        public static async Task<JObject> ParseResponseFromUrl(string url)
        {
            var httpClient = HttpClientFactory.Create();
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JObject.Parse(content);
        }

        public static async Task<JObject> ConvertToOurApiFormat(JObject parsedBody)
        {
            if (parsedBody == null)
                throw new ArgumentNullException(nameof(parsedBody));

            var newBody = new JObject(parsedBody);

            var results = (JArray)newBody["results"];

            for (var i = 0; i < results.Count; i++)
            {
                var eventInfo = (JObject)results[i];

                var imagesInfo = eventInfo["images"];

                var links = imagesInfo.Select(info => (string)info["image"]).ToList();

                eventInfo.Property("images").Remove();

                eventInfo["images"] = new JArray(links);

                var placeId = (long)eventInfo["place"]["id"];

                var place = await GetPlaceById(placeId);

                eventInfo.Property("place").Value = JToken.FromObject(place, SnakeCaseSerializer);

                var (start, end) = GetDatesFromJson((JArray)eventInfo["dates"]);

                eventInfo.Property("dates").Remove();

                eventInfo["start"] = JToken.FromObject(start);
                eventInfo["end"] = JToken.FromObject(end);
            }

            return newBody;
        }

        private static (DateTime, DateTime) GetDatesFromJson(JArray dates)
        {
            //для простоты будем пока работать с первой записью в массиве дат,
            //т.е. не будем учитывать регулярные мероприятия

            var firstEntry = dates[0];
            var startTimestamp = (long)firstEntry["start"];
            var endTimestamp = (long)firstEntry["end"];

            var start = DateTimeOffset.FromUnixTimeSeconds(startTimestamp).ToLocalTime().DateTime;
            var end = DateTimeOffset.FromUnixTimeSeconds(endTimestamp).ToLocalTime().DateTime;

            return (start, end);
        }

        public static async Task<KudagoPlace> GetPlaceById(long placeId)
        {
            if (!PlacesInfoCache.ContainsKey(placeId))
            {
                var body = await ParseResponseFromUrl(string.Format(Constants.PlaceDetailsUrlPattern, placeId));

                var place = body.ToObject<KudagoPlace>();

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
