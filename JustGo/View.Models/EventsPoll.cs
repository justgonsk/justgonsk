using System;
using System.Collections.Generic;
using JustGo.Extern.Models;

namespace JustGo.View.Models
{
    public class EventsPoll
    {
        public long Count { get; set; }
        public List<KudagoEvent> Results { get; set; }
    }
}