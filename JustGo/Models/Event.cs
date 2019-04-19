﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using JustGo.Helpers;
using JustGo.Interfaces;
using JustGo.View.Models;
using Newtonsoft.Json;

namespace JustGo.Models
{
    /// <remarks>
    /// Виртуальные свойства используются для lazy loading
    /// </remarks>
    public class Event : IConvertibleToViewModel<EventViewModel>
    {
        /// <summary>
        /// Первичный ключ в нашей базе.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Навигационное свойство <see cref="Place"/> и внешний ключ <see cref="PlaceId"/>
        /// </summary>
        public Place Place { get; set; }

        /// <summary>
        /// Навигационное свойство <see cref="Place"/> и внешний ключ <see cref="PlaceId"/>
        /// </summary>
        public int PlaceId { get; set; }

        [Required]
        public string Title { get; set; }

        public string ShortTitle { get; set; }

        [Required]
        public string Description { get; set; }

        public ICollection<EventCategory> EventCategories { get; set; }

        public ICollection<EventTag> EventTags { get; set; }

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

        public EventViewModel ToViewModel()
        {
            return new EventViewModel
            {
                Id = Id,
                Title = Title,
                ShortTitle = ShortTitle,
                Description = Description,
                Dates = new List<EventDate>(Dates),
                Images = new List<ImageModel>(Images),
                Categories = EventCategories.Select(eventCat => eventCat.Category.Name).ToList(),
                Tags = EventTags.Select(eventTag => eventTag.Tag.Name).ToList(),
                Place = Place.ToViewModel()
            };
        }
    }
}