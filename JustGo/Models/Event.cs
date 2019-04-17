using System;
using System.Collections.Generic;
using System.Linq;
using JustGo.Helpers;
using JustGo.View.Models;

namespace JustGo.Models
{
    public class Event : IConvertibleToViewModel<EventViewModel>
    {
        /// <summary>
        /// Первичный ключ в нашей базе.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Навигационное свойство Place и внешний ключ PlaceId
        /// </summary>
        public Place Place { get; set; }

        /// <summary>
        /// Навигационное свойство Place и внешний ключ PlaceId
        /// </summary>
        public int PlaceId { get; set; }

        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Description { get; set; }
        public ICollection<string> Categories { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<ImageModel> Images { get; set; }
        public ICollection<EventDate> Dates { get; set; }

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

        public EventViewModel ConvertToViewModel()
        {
            return new EventViewModel
            {
                Title = Title,
                ShortTitle = ShortTitle,
                Description = Description,
                Dates = new List<EventDate>(Dates),
                Images = new List<ImageModel>(Images),
                Categories = new List<string>(Categories),
                Tags = new List<string>(Tags),
                Place = Place.ConvertToViewModel()
            };
        }
    }
}