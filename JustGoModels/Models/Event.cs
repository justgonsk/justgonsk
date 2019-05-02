using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using JustGoModels.Interfaces;
using JustGoModels.Models.View;

namespace JustGoModels.Models
{
    /// <remarks>
    /// Виртуальные свойства используются для lazy loading
    /// </remarks>
    public class Event : IConvertibleToViewModel<EventViewModel>
    {
        /// <summary>
        /// Первичный ключ в нашей базе.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Навигационное свойство <see cref="Place"/> и внешний ключ <see cref="PlaceId"/>
        /// </summary>
        public virtual Place Place { get; set; }

        /// <summary>
        /// Навигационное свойство <see cref="Place"/> и внешний ключ <see cref="PlaceId"/>
        /// </summary>
        public int PlaceId { get; set; }

        public string Title { get; set; }

        public string ShortTitle { get; set; }

        public string BodyText { get; set; }

        public string Description { get; set; }

        public virtual ICollection<EventCategory> EventCategories { get; set; }

        public virtual ICollection<EventTag> EventTags { get; set; }

        public virtual ICollection<ImageModel> Images { get; set; }

        public List<SingleDate> SingleDates { get; set; }

        public List<ScheduledDate> ScheduledDates { get; set; }

        /// <summary>
        /// Даты (начало и конец) текущего проведения этого мероприятия
        /// Если такого нет, вернёт null
        /// </summary>
        public SingleDate FindCurrent() => FindCurrentInSingleDates() ?? FindCurrentInScheduledDates();

        private SingleDate FindCurrentInSingleDates()
        {
            var now = DateTime.Now;
            return SingleDates?.FirstOrDefault(sd => sd.Start >= now && sd.End <= now);
        }

        private SingleDate FindCurrentInScheduledDates()
        {
            var now = DateTime.Now;
            var currentScheduledDate = ScheduledDates?
                .FirstOrDefault(sd => sd.ScheduleStart >= now && sd.ScheduleEnd <= now);
            if (currentScheduledDate == null)
            {
                return null;
            }

            var currentDay = (int)now.DayOfWeek;
            var currentTime = now.TimeOfDay;

            var currentSchedule = currentScheduledDate.Schedules?
                .Where(sch => sch.DaysOfWeek.Contains(currentDay))
                .FirstOrDefault(schedule =>
                {
                    return schedule.EndTime != null && schedule.StartTime != null
                           && schedule.StartTime.Value >= currentTime
                           && schedule.EndTime.Value <= currentTime;
                });

            if (currentSchedule == null)
            {
                return null;
            }

            var start = DateTime.Today.AddHours(currentSchedule.StartTime?.Hours ?? 0);
            var end = DateTime.Today.AddHours(currentSchedule.EndTime?.Hours ?? 0);

            return new SingleDate(start, end);
        }

        /// <summary>
        /// Даты (начало и конец) следующего ближайшего проведения этого мероприятия
        /// Если в будущем не запланировано (до максимального срока), вернёт null
        /// </summary>
        /// <param name="from">Минимальный срок с которого пытаемся искать</param>
        /// <param name="to">Максимальный срок до которого пытаемся искать</param>
        /// <returns></returns>
        public SingleDate FindFirstInRange(DateTime from, DateTime to)
        {
            var inSingleDates = FindFirstInRangeInSingleDates(from, to);
            var inScheduledDates = FindFirstInRangeInScheduledDates(from, to);
            return new[] { inSingleDates, inScheduledDates }
                .Where(date => date != null)
                .OrderBy(date => date.Start)
                .FirstOrDefault();
        }

        private SingleDate FindFirstInRangeInSingleDates(DateTime from, DateTime to)
        {
            return SingleDates?
                .OrderBy(date => date.Start)
                .ThenBy(date => date.End)
                .FirstOrDefault(sd => sd.Start >= from && sd.End <= to);
        }

        private SingleDate FindFirstInRangeInScheduledDates(DateTime from, DateTime to)
        {
            var currentDay = (int)from.DayOfWeek;

            var scheduledDate = ScheduledDates?
                .OrderBy(date => date.ScheduleStart)
                .ThenBy(date => date.ScheduleEnd)
                .FirstOrDefault(sd => sd.ScheduleStart <= from
                                      && sd.ScheduleEnd >= to);

            if (scheduledDate == null)
            {
                return null;
            }

            var next = FindFirstInRangeInCurrentDay(scheduledDate, currentDay, from, to);

            if (next != null)
            {
                return next;
            }

            for (int i = (currentDay + 1) % 7, j = 1; i != currentDay; i = (i + 1) % 7, j++)
            {
                //начало очередного следующего дня
                var fromWithOffset = from.Date.AddDays(j);

                next = FindFirstInRangeInAnotherDay(scheduledDate, i, fromWithOffset, to);
                if (next != null)
                {
                    break;
                }
            }

            return next;
        }

        private SingleDate FindFirstInRangeInCurrentDay(ScheduledDate scheduledDate,
            int currentDay, DateTime from, DateTime to)
        {
            var minimumTime = from.TimeOfDay;
            var nextSchedule = scheduledDate.Schedules?
                .Where(sch => sch.DaysOfWeek.Contains(currentDay))
                .OrderBy(sch => sch.StartTime)
                .FirstOrDefault(sch => sch.StartTime >= minimumTime);

            if (nextSchedule == null)
            {
                return null;
            }

            var start = from.Date.Add(nextSchedule.StartTime ?? TimeSpan.Zero);
            var end = to.Date.Add(nextSchedule.EndTime ?? TimeSpan.FromDays(1));

            return to >= end ? new SingleDate(start, end) : null;
        }

        private SingleDate FindFirstInRangeInAnotherDay(ScheduledDate scheduledDate,
            int dayNumber, DateTime fromWithOffset, DateTime to)
        {
            var nextSchedule = scheduledDate.Schedules?
                .Where(sch => sch.DaysOfWeek.Contains(dayNumber))
                .OrderBy(sch => sch.StartTime)
                .FirstOrDefault();

            if (nextSchedule == null)
            {
                return null;
            }

            var start = fromWithOffset.Add(nextSchedule.StartTime ?? TimeSpan.Zero);
            var end = fromWithOffset.Add(nextSchedule.EndTime ?? TimeSpan.FromDays(1));

            return to >= end ? new SingleDate(start, end) : null;
        }

        public EventViewModel ToViewModel()
        {
            var now = DateTime.Now;

            return new EventViewModel
            {
                Id = Id,
                Title = Title,
                ShortTitle = ShortTitle,
                Description = Description,
                BodyText = BodyText,
                SingleDates = SingleDates,
                ScheduledDates = ScheduledDates,
                Images = Images,
                Categories = EventCategories?.Select(eventCat => eventCat.Category.Name).ToList(),
                Tags = EventTags?.Select(eventTag => eventTag.Tag.Name).ToList(),
                Place = Place?.ToViewModel(),
                Current = FindCurrent(),
                NextOnWeek = FindFirstInRange(now, now.AddDays(7))
            };
        }
    }
}