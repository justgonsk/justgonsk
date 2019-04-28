using JustGoModels.Models.View;
using Newtonsoft.Json;

namespace JustGoModels.Models.Edit
{
    /// <summary>
    /// Класс для инициализации новых значений у viewmodel
    /// Свойства могут быть null - это значит что мы их не меняем.
    /// </summary>
    public class PlaceEditModel
    {
        public int? Id { get; set; }

        public string Title { get; set; }

        public string Address { get; set; }

        [JsonProperty("coords")]
        public Coordinates Coordinates { get; set; }

        public static implicit operator PlaceViewModel(PlaceEditModel model)
        {
            return new PlaceViewModel
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