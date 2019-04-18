using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JustGo.Models;

namespace JustGo.View.Models
{
    public class EventViewModel : IConvertibleToModel<Event>
    {
        public int? Id { get; set; }

        [Required, MinLength(3), MaxLength(30)]
        public string Title { get; set; }

        public string ShortTitle { get; set; }

        [Required, MinLength(3), MaxLength(10000)]
        public string Description { get; set; }

        public PlaceViewModel Place { get; set; } // место проведения
        public List<string> Categories { get; set; } // список категорий
        public List<string> Tags { get; set; }
        public List<ImageModel> Images { get; set; }

        public List<EventDate> Dates { get; set; }

        public Event ToModel()
        {
            return new Event
            {
                Title = Title,
                ShortTitle = ShortTitle,
                Description = Description,
                Dates = new List<EventDate>(Dates),
                Images = new List<ImageModel>(Images)
            };
        }
    }
}