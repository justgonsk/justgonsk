using JustGo.Models;
using Newtonsoft.Json;

namespace JustGo.View.Models.Edit
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
    }
}