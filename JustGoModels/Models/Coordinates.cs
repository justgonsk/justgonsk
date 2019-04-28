using GeoCoordinatePortable;
using Newtonsoft.Json;

namespace JustGoModels.Models
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