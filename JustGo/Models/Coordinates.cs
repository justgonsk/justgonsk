using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JustGo.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Coordinates : GeoCoordinate
    {
        [JsonProperty("lon")]
        public new double Longitude
        {
            get => base.Longitude;

            set => base.Longitude = value;
        }

        [JsonProperty("lat")]
        public new double Latitude
        {
            get => base.Latitude;

            set => base.Latitude = value;
        }
    }
}