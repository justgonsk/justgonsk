using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JustGoModels.Models
{
    public class ScheduledDate : IValidatableObject
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

        /// <summary>
        /// Выполняет проверку, что дата начала раньше даты конца
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Перечисление из одного элемента в случае ошибки</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var current = (ScheduledDate)context.ObjectInstance;
            if (current.ScheduleEnd < current.ScheduleStart)
            {
                yield return new ValidationResult("Дата начала должна быть раньше даты конца!",
                    new[] { nameof(ScheduleStart), nameof(ScheduleEnd) });
            }
        }
    }
}