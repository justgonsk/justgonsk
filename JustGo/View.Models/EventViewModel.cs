using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JustGo.Models;
using Newtonsoft.Json;

namespace JustGo.View.Models
{
    public class EventViewModel
    {
        public int? Id { get; set; }

        [Required, MinLength(3), MaxLength(300)]
        public string Title { get; set; }

        public string ShortTitle { get; set; }

        [Required, MinLength(3), MaxLength(10000)]
        public string Description { get; set; }

        public PlaceViewModel Place { get; set; } // место проведения
        public List<string> Categories { get; set; } // список категорий
        public List<string> Tags { get; set; }
        public List<ImageModel> Images { get; set; }

        public List<EventDate> Dates { get; set; }

        public bool IsSingle => Dates.Count == 1;

        /// <summary>
        /// Даты (начало и конец) последнего проведения этого мероприятия
        /// Если ещё не проводилось, вернёт null
        /// </summary>
        public EventDate Latest
        {
            get
            {
                return Dates
                    .OrderBy(date => date)
                    .LastOrDefault(date => date.ActualEnd < DateTime.Now);
            }
        }

        /// <summary>
        /// Даты (начало и конец) следующего ближайшего проведения этого мероприятия
        /// Если в будущем не запланировано, вернёт null
        /// </summary>
        public EventDate NearestNext
        {
            get
            {
                return Dates
                    .OrderBy(date => date)
                    .FirstOrDefault(date => date.ActualEnd > DateTime.Now);
            }
        }
    }
}