using System;
using System.Linq;
using JustGoModels.Interfaces;

namespace JustGoModels.Models.View
{
    /// <summary>
    /// Информация для создания фильтра. Передаётся в запросе
    /// </summary>
    public class EventsFilterViewModel : IConvertibleToModel<EventsFilter>
    {
        private const char Separator = ';';

        /// <summary>
        /// Список требуемых категорий в строковом представлении.
        /// Если null, тогда не фильтруем по категориям
        /// Если пустой, ищем только события без категорий
        /// </summary>
        public string Categories { get; set; }

        /// <summary>
        /// Список требуемых тэгов в строковом представлении.
        /// Если null, тогда не фильтруем по тэгам
        /// Если пустой, ищем только события без тэгов
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Список ID допустимых мест.
        /// Если null, тогда не фильтруем по местам
        /// Если пустой, ищем события где место не указано
        /// </summary>
        public string Places { get; set; }

        /// <summary>
        /// Если указано, то события раньше этого момента включаться не будут
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Если указано, то события позже этого момента включаться не будут
        /// </summary>
        public DateTime? To { get; set; }

        public EventsFilter ToModel()
        {
            return new EventsFilter
            {
                RequiredCategories = Categories?.Split(Separator).ToList(),
                RequiredTags = Tags?.Split(Separator).ToList(),

                AllowedPlaceIds = Places?.Split(Separator).Select(x =>
                {
                    if (int.TryParse(x, out int result))
                    {
                        return result;
                    }
                    else
                    {
                        throw new ArgumentException("Place IDs must be ints", nameof(Places));
                    }
                }).ToList(),

                From = From,
                To = To
            };
        }
    }
}