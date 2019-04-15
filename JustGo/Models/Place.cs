using System;
//using System.Device.Location;
using Newtonsoft.Json;

namespace JustGo.Models
{
    public class Place
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }

        [JsonProperty("coords")]
        public Coordinates Coordinates { get; set; }
    }
}
