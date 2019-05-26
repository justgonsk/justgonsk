using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JustGoModels.Models
{
    public class Schedule : IValidatableObject
    {
        public List<int> DaysOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// Выполняет проверку, что время начала раньше времени конца
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Перечисление из одного элемента в случае ошибки</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var current = (Schedule)context.ObjectInstance;
            if (current.EndTime < current.StartTime)
            {
                yield return new ValidationResult("Время начала должно быть раньше времени конца!",
                    new[] { nameof(StartTime), nameof(EndTime) });
            }
        }
    }
}