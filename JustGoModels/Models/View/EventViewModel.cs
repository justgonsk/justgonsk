using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JustGoModels.Models.Edit;

namespace JustGoModels.Models.View
{
    public class EventViewModel
    {
        public int? Id { get; set; }

        [Required, MinLength(3), MaxLength(300)]
        public string Title { get; set; }

        public string ShortTitle { get; set; }

        [Required, MinLength(3), MaxLength(10000)]
        public string Description { get; set; }

        public string BodyText { get; set; }

        public PlaceEditModel Place { get; set; } // место проведения
        public List<string> Categories { get; set; } // список категорий
        public List<string> Tags { get; set; }
        public ICollection<ImageModel> Images { get; set; }

        public ICollection<SingleDate> SingleDates { get; set; }
        public ICollection<ScheduledDate> ScheduledDates { get; set; }

        public SingleDate Current { get; set; }
        public SingleDate NextOnWeek { get; set; }
    }
}