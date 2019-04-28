using System.ComponentModel.DataAnnotations;
using JustGoModels.Models.Edit;
using Newtonsoft.Json;

namespace JustGoModels.Models.View
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

        public static implicit operator PlaceEditModel(PlaceViewModel model)
        {
            return new PlaceEditModel
            {
                Id = model.Id,
                Title = model.Title,
                Address = model.Address,
                Coordinates = new Coordinates
                {
                    Latitude = model.Coordinates.Latitude,
                    Longitude = model.Coordinates.Longitude
                }
            };
        }
    }
}