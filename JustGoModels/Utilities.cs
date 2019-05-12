using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JustGoModels
{
    public static class Utilities
    {
        public static readonly TimeZoneInfo novosibirskTimeZone =
            TimeZoneInfo.GetSystemTimeZones().First(zone =>
            {
                var zoneId = zone.Id;
                return zoneId.Contains("Novosibirsk") || zoneId.Contains("N. Central");
            });

        public static DateTime NovosibirskNow
            => TimeZoneInfo.ConvertTime(DateTime.Now,
                TimeZoneInfo.Local, novosibirskTimeZone);

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

        public static bool IsInRange(this DateTime dateTime, DateTime lower, DateTime upper)
        {
            return lower < dateTime && upper > dateTime;
        }
    }
}