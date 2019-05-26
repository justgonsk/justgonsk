using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JustGoModels.Models.Edit;

namespace JustGoModels.Models.View
{
    //Это видимо не понадобится, но пусть будет, на всякий
    public class EventInstance
    {
        public int? Id { get; set; }

        [Required, MinLength(3), MaxLength(300)]
        public string Title { get; set; }

        public string ShortTitle { get; set; }

        [Required, MinLength(3), MaxLength(10000)]
        public string Description { get; set; }

        public PlaceEditModel Place { get; set; } // место проведения
        public List<string> Categories { get; set; } // список категорий
        public List<string> Tags { get; set; }
        public List<ImageModel> Images { get; set; }

        public List<EventDate> Dates { get; set; }

        public bool IsSingle => Dates.Count == 1;

        /// <summary>
        /// Даты (начало и конец) текущего проведения этого мероприятия
        /// Если такого нет, вернёт null
        /// </summary>
        public EventDate Current
        {
            get
            {
                return Dates
                    .OrderBy(date => date.ActualStart)
                    .FirstOrDefault(date =>
                    {
                        var startBeforeNow = DateTime.Now > date.ActualStart;
                        var endAfterNow = DateTime.Now < date.ActualEnd;
                        return startBeforeNow && endAfterNow;
                    });
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
                    .OrderBy(date => date.ActualStart)
                    .FirstOrDefault(date => date.ActualStart > DateTime.Now);
            }
        }
    }
}