﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace JustGo.Models
{
    public class EventDate : IValidatableObject
    {
        /// <summary>
        /// Суррогатный ключ для базы
        /// </summary>
        [JsonIgnore]
        public int EventDateId { get; set; }

        /// <summary>
        /// Навигационное свойство Event и внешний ключ EventId
        /// </summary>
        [JsonIgnore]
        public virtual Event Event { get; set; }

        /// <summary>
        /// Навигационное свойство Event и внешний ключ EventId
        /// </summary>
        [JsonIgnore]
        public int EventId { get; set; }

        /// <summary>
        /// Момент начала.
        /// </summary>
        [NullOrRange(typeof(DateTime), "01/01/2014", "01/01/2021")]
        public DateTime? Start { get; set; }

        /// <summary>
        /// Момент конца.
        /// </summary>
        [NullOrRange(typeof(DateTime), "01/01/2014", "01/01/2021")]
        public DateTime? End { get; set; }

        /// <summary>
        /// Если End указано, то событие завершилось в момент End.
        /// Если End не указано, то событие завершилось в момент Start.
        /// Если ни End, ни Start не указано, то событие завершилось в момент DateTime.MaxValue.
        /// Это вспомогательное свойство для внутренних нужд. Не сериализуется в ответах
        /// </summary>
        [JsonIgnore]
        public DateTime ActualEnd => End ?? Start ?? DateTime.MaxValue;

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
                    new[] { nameof(Start), nameof(End) });
            }
        }
    }

    public class NullOrRangeAttribute : RangeAttribute
    {
        public NullOrRangeAttribute(Type type, string minimum, string maximum)
            : base(type, minimum, maximum)
        {
        }

        public override bool IsValid(object value)
        {
            return value == null || base.IsValid(value);
        }
    }
}