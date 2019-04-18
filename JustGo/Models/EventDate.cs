using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JustGo.Models
{
    public class EventDate : IValidatableObject
    {
        /// <summary>
        /// Навигационное свойство Event и внешний ключ EventId
        /// </summary>
        public Event Event { get; set; }

        /// <summary>
        /// Навигационное свойство Event и внешний ключ EventId
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Момент начала. Должно быть указано всегда
        /// </summary>
        [Required]
        [Range(typeof(DateTime), "01/01/2010", "01/01/2021")]
        public DateTime Start { get; set; }

        /// <summary>
        /// Момент конца. Может не указываться, тогда считаем что после момента Start
        /// мероприятие неактуально.
        /// </summary>
        [Range(typeof(DateTime), "01/01/2010", "01/01/2021")]
        public DateTime? End { get; set; }

        /// <summary>
        /// Если End указано, то событие завершилось в момент End.
        /// Если End не указано, то событие завершилось в момент Start.
        /// </summary>
        public DateTime ActualEnd => End ?? Start;

        /// <summary>
        /// Закончилось мероприятие в текущий момент или нет
        /// </summary>
        public bool HasEnded => ActualEnd < DateTime.Now;

        /// <summary>
        /// Выполняет проверку, что дата начала раньше даты конца
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Перечисление из одного элемента в случае ошибки</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var eventDate = (EventDate)context.ObjectInstance;
            if (eventDate.End.HasValue && eventDate.End < eventDate.Start)
            {
                yield return new ValidationResult("Дата начала должна быть раньше даты конца!",
                    new[]
                    {
                        nameof(Start), nameof(End)
                    });
            }
        }
    }
}