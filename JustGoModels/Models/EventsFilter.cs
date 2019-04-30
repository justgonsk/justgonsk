using System;
using System.Collections.Generic;
using System.Linq;
using JustGoModels.Models.View;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JustGoModels.Models
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
        public List<string> RequiredCategories { get; set; }

        /// <summary>
        /// Список требуемых тэгов в строковом представлении.
        /// Если null, тогда не фильтруем по тэгам
        /// Если пустой, ищем только события без тэгов
        /// </summary>
        public List<string> RequiredTags { get; set; }

        /// <summary>
        /// Список ID допустимых мест.
        /// Если null, тогда не фильтруем по местам
        /// Если пустой, ищем события где место не указано
        /// </summary>
        public List<int> AllowedPlaceIds { get; set; }

        /// <summary>
        /// Если указано, то события раньше этого момента включаться не будут
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Если указано, то события позже этого момента включаться не будут
        /// </summary>
        public DateTime? To { get; set; }

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
            return AllowedPlaceIds == null || AllowedPlaceIds.Contains(@event.Place.Id);
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
            var placeId = @event.Place.Id;
            return placeId != null &&
                   (AllowedPlaceIds == null || AllowedPlaceIds.Contains(placeId.Value));
        }
    }
}