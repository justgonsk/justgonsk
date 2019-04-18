using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JustGo.Models;
using Newtonsoft.Json;

namespace JustGo.View.Models
{
    public class PlaceViewModel : IConvertibleToModel<Place>
    {
        /// <summary>
        /// Если указан, будет сопоставляться с первичным ключом в нашей таблице
        /// </summary>
        public int? Id { get; set; }

        public string Title { get; set; }

        public string Address { get; set; }

        [JsonProperty("coords")]
        public Coordinates Coordinates { get; set; }

        public Place ToModel()
        {
            return new Place
            {
                Title = Title,
                Address = Address,
                Coordinates = new Coordinates
                {
                    Latitude = Coordinates.Latitude,
                    Longitude = Coordinates.Longitude
                }
            };
        }
    }
}