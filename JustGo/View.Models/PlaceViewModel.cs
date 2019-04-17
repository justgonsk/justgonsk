using System.ComponentModel.DataAnnotations;
using JustGo.Models;
using Newtonsoft.Json;

namespace JustGo.View.Models
{
    public class PlaceViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Address { get; set; }

        [JsonProperty("coords")]
        public Coordinates Coordinates { get; set; }
    }
}