using System;
using System.Collections.Generic;

namespace JustGoModels.Models
{
    public class Schedule
    {
        public List<int> DaysOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}