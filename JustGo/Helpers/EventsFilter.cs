using System;
using System.Collections.Generic;
using System.Linq;
using JustGo.Extern.Models;
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
        /// Список допустимых категорий в строковом представлении. Если null, тогда не фильтруем по категориям
        /// </summary>
        public List<string> Categories { get; set; }

        /// <summary>
        /// Список допустимых тэгов в строковом представлении. Если null, тогда не фильтруем по тэгам
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Список допустимых категорий в виде view-моделек. Если null, тогда не фильтруем по местам
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

        public IEnumerable<T> FilterEvents<T>(IEnumerable<T> events) where T : EventViewModel
        {
            var eventViewModels = FilterEvents(events.Select(vm => vm.ToModel())).Select(m => m.ToViewModel());
            return eventViewModels as IEnumerable<T>;
        }

        private bool SatisfiesFilter(Event @event)
        {
            return PlaceIsFromFilter(@event)
                   && HasCategories(@event)
                   && HasTags(@event);
        }

        private bool HasCategories(Event @event)
        {
            if (Categories == null)
            {
                return true;
            }

            foreach (var category in @event.EventCategories.Select(eventCat => eventCat.Category.Name))
            {
                if (!Categories.Contains(category))
                {
                    return false; // если нет хотя бы одной категории
                }
            }

            return true;
        }

        private bool HasTags(Event @event)
        {
            if (Tags == null)
            {
                return true;
            }

            foreach (var tag in @event.EventTags.Select(eventTag => eventTag.Tag.Name))
            {
                if (!Tags.Contains(tag))
                {
                    return false; // если нет хотя бы одной категории
                }
            }

            return true;
        }

        private bool PlaceIsFromFilter(Event @event)
        {
            if (Places == null)
            {
                return true;
            }

            return Places.Contains(@event.Place.ToViewModel());
        }
    }
}