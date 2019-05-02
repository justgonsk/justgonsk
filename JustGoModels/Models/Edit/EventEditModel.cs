using System.Collections.Generic;

namespace JustGoModels.Models.Edit
{
    /// <summary>
    /// Класс для инициализации новых значений у viewmodel
    /// Свойства могут быть null - это значит что мы их не меняем.
    /// </summary>
    public class EventEditModel
    {
        public int? Id { get; set; }

        public string Title { get; set; }

        public string ShortTitle { get; set; }

        public string Description { get; set; }

        public PlaceEditModel Place { get; set; } // место проведения
        public List<string> Categories { get; set; } // список категорий
        public List<string> Tags { get; set; }
        public List<ImageModel> Images { get; set; }

        public List<SingleDate> SingleDates { get; set; }
        public List<ScheduledDate> ScheduledDates { get; set; }
    }
}