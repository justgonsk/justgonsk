using System;
using System.Net.Http;
using System.Threading.Tasks;
using JustGoModels.Models.View;
using JustGoUtilities;
using Newtonsoft.Json.Linq;
using Models;

namespace VKManager.ResponseHelpers
{
    public static class ResponseHelper
    {
        private static string urlPattern = "https://api.vk.com/method/groups.search?q={0}" +
        	"&access_token=5604cd62e4baff93200099fc1bdd4704f90415f35f29f299b157f4f87d836524c3b21b72504a97bf19e2d&v=5.92" +
        	"&city_id=99&count=900&future=1&type=event";

        public static async Task<Poll<VKEventModel>> GetAllEventsFromTarget()
        {
            var httpClient = new HttpClient();

            var url = string.Format(urlPattern, Constants.DefaultVKSearchQuery);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JObject.Parse(content);
        }
    }
}
