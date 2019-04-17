using System;
using System.Collections.Generic;
using System.Linq;
using JustGo.Models;

namespace JustGo.View.Models
{
    public class EventsPoll
    {
        public long Count { get; set; }
        public List<Event> Results { get; set; }

        public void FilterBy(EventsFilter filter)
        {
            // TODO: сделать код более масштабируемым
            Results = Results.Where(x => x.HasCategories(filter.Categories)).ToList();
            Count = Results.Count;
        }
    }
}
