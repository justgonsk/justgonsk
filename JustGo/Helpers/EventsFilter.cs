using System;
using System.Collections.Generic;
using System.Linq;

using JustGo.Interfaces;
using JustGo.Models;
using JustGo.View.Models;
using Newtonsoft.Json;

namespace JustGo.Helpers
{
    /// <summary>
    /// Фильтр, отсеивающий события
    /// </summary>
    public class EventsFilter
    {
        /// <summary>
        /// Список требуемых категорий в строковом представлении.
        /// Если null, тогда не фильтруем по категориям
        /// Если пустой, ищем только события без категорий
        /// </summary>
        [JsonProperty("categories")]
        public List<string> RequiredCategories { get; set; }

        /// <summary>
        /// Список требуемых тэгов в строковом представлении.
        /// Если null, тогда не фильтруем по тэгам
        /// Если пустой, ищем только события без тэгов
        /// </summary>
        [JsonProperty("tags")]
        public List<string> RequiredTags { get; set; }

        /// <summary>
        /// Список допустимых мест в виде view-моделек.
        /// Если null, тогда не фильтруем по местам
        /// Если пустой, ищем события где место не указано
        /// </summary>
        [JsonProperty("places")]
        public List<PlaceViewModel> AllowedPlaces { get; set; }


        /// <summary>
        /// Список ID допустимых мест.
        /// Если null, тогда не фильтруем по местам
        /// Если пустой, ищем события где место не указано
        /// </summary>
        [JsonProperty("placeIds")]
        public List<int> AllowedPlacesIds { get; set; }

        public void ParseParameters(string categories, string tags, string places)
        {
            RequiredCategories = categories?.Split(',').ToList();
            RequiredTags = tags?.Split(',').ToList();

            AllowedPlacesIds = places?.Split(',').Select(x =>
            {
                if (int.TryParse(x, out int result))
                {
                    return result;
                }
                else
                {
                    throw new ArgumentException("Place IDs must be numbers", nameof(places));
                }
            }).ToList();

            AllowedPlaces = null;
        }

        public IEnumerable<Event> FilterEvents(IEnumerable<Event> sequence)
        {
            return sequence.Where(SatisfiesFilter);
        }

        public IEnumerable<EventViewModel> FilterEvents(IEnumerable<EventViewModel> sequence)
        {
            return sequence.Where(SatisfiesFilter);
        }

        public bool SatisfiesFilter(Event @event)
        {
            return PlaceIsFromFilter(@event)
                   && HasCategories(@event)
                   && HasTags(@event);
        }

        private bool HasCategories(Event @event)
        {
            var eventCategories = @event.EventCategories
                .Select(eventCategory => eventCategory.Category.Name);

            return RequiredCategories == null || RequiredCategories
                       .All(filterCategory => eventCategories
                       .Contains(filterCategory));
        }

        private bool HasTags(Event @event)
        {
            var eventTags = @event.EventTags
                .Select(eventTag => eventTag.Tag.Name);

            return RequiredTags == null || RequiredTags
                .All(filterTag => eventTags
                .Contains(filterTag));
        }

        private bool PlaceIsFromFilter(Event @event)
        {
            return AllowedPlaces == null || AllowedPlaces.Contains(@event.Place.ToViewModel());
        }

        public bool SatisfiesFilter(EventViewModel @event)
        {
            return PlaceIsFromFilter(@event)
                   && HasCategories(@event)
                   && HasTags(@event);
        }

        private bool HasCategories(EventViewModel @event)
        {
            return RequiredCategories == null || RequiredCategories
                   .All(requiredCategory => @event.Categories.Contains(requiredCategory));
        }

        private bool HasTags(EventViewModel @event)
        {
            return RequiredTags == null || RequiredTags
                   .All(requiredTag => @event.Tags.Contains(requiredTag));
        }

        private bool PlaceIsFromFilter(EventViewModel @event)
        {
            return AllowedPlaces == null || AllowedPlaces.Contains(@event.Place);
        }
    }
}