using System;
using System.Collections.Generic;

namespace JustGo.Models
{
    public class EventsPoll
    {
        public long Id { get; set; }
        public long Count { get; set; }
        public List<Event> Results { get; set; }
    }
}
