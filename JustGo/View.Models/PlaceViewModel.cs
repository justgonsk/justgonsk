using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JustGo.Models;
using Newtonsoft.Json;

namespace JustGo.View.Models
{
    public class PlaceViewModel
    {
        /// <summary>
        /// Если указан, будет сопоставляться с первичным ключом в нашей таблице
        /// </summary>
        public int? Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Address { get; set; }

        [JsonProperty("coords")]
        public Coordinates Coordinates { get; set; }
    }
}