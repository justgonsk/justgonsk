using System;
using System.Collections.Generic;

namespace JustGoModels.Models
{
    public class ScheduledDate
    {
        public DateTime ScheduleStart { get; }
        public DateTime ScheduleEnd { get; }
        public List<Schedule> Schedules { get; }

        public ScheduledDate(DateTime scheduleStart, DateTime scheduleEnd, List<Schedule> schedules)
        {
            ScheduleStart = scheduleStart;
            ScheduleEnd = scheduleEnd;
            Schedules = schedules;
        }
    }
}