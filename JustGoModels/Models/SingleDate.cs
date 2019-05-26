using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JustGoModels.Models
{
    public class SingleDate : IValidatableObject
    {
        [Range(typeof(DateTime), "01/01/2019", "01/01/2021",
            ErrorMessage = "Дата должна быть от 01.01.2019 до 01.01.2021")]
        public DateTime Start { get; }

        [Range(typeof(DateTime), "01/01/2019", "01/01/2021",
            ErrorMessage = "Дата должна быть от 01.01.2019 до 01.01.2021")]
        public DateTime End { get; }

        public SingleDate(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Выполняет проверку, что дата начала раньше даты конца
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Перечисление из одного элемента в случае ошибки</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var current = (SingleDate)context.ObjectInstance;
            if (current.End < current.Start)
            {
                yield return new ValidationResult("Дата начала должна быть раньше даты конца!",
                    new[] { nameof(Start), nameof(End) });
            }
        }
    }
}