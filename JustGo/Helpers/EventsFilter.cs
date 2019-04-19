using System;
using System.Collections.Generic;
using System.Linq;

using JustGo.Interfaces;
using JustGo.Models;
using JustGo.View.Models;

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
        public List<string> RequiredCategories { get; set; }

        /// <summary>
        /// Список требуемых тэгов в строковом представлении.
        /// Если null, тогда не фильтруем по тэгам
        /// Если пустой, ищем только события без тэгов
        /// </summary>
        public List<string> RequiredTags { get; set; }

        /// <summary>
        /// Список допустимых мест в виде view-моделек.
        /// Если null, тогда не фильтруем по местам
        /// Если пустой, ищем события где место не указано
        /// </summary>
        public List<PlaceViewModel> Places { get; set; }

        /// <summary>
        /// Перечисляет удовлетворяющие фильтру события, пока это требуется или пока они не закончились
        /// </summary>
        /// <param name="repository">Хранилище событий</param>
        /// <returns></returns>
        public IEnumerable<Event> FilterEvents(IEventsRepository repository)
        {
            return FilterEvents(repository.EnumerateAll());
        }

        public IEnumerable<Event> FilterEvents(IEnumerable<Event> sequence)
        {
            return sequence.Where(SatisfiesFilter);
        }

        public IEnumerable<EventViewModel> FilterEvents(IEnumerable<EventViewModel> sequence)
        {
            return sequence.Where(SatisfiesFilter);
        }

        private bool SatisfiesFilter(Event @event)
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
            return Places == null || Places.Contains(@event.Place.ToViewModel());
        }

        private bool SatisfiesFilter(EventViewModel @event)
        {
            return PlaceIsFromFilter(@event)
                   && HasCategories(@event)
                   && HasTags(@event);
        }

        private bool HasCategories(EventViewModel @event)
        {
            return RequiredCategories == null || RequiredCategories
                   .All(filterCategory => @event.Categories.Contains(filterCategory));
        }

        private bool HasTags(EventViewModel @event)
        {
            return RequiredTags == null || RequiredTags
                   .All(filterTag => @event.Tags.Contains(filterTag));
        }

        private bool PlaceIsFromFilter(EventViewModel @event)
        {
            return Places == null || Places.Contains(@event.Place);
        }
    }
}