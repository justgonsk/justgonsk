using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGo.Data;
using JustGo.Helpers;
using JustGo.Interfaces;
using JustGo.Models;
using JustGo.View.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace JustGo.Repositories
{
    public class DbEventsRepository : IEventsRepository
    {
        private readonly MainContext context;

        public DbEventsRepository(MainContext context)
        {
            this.context = context;
        }

        public IEnumerable<Event> EnumerateAll() => context.Events.AsNoTracking();

        public Poll<EventViewModel> GetEventPoll(EventsFilter filter = null,
            int? offset = 0, int? count = 100)
        {
            var wholeSequence = filter?.FilterEvents(this) ?? EnumerateAll();

            var limitedSequence = wholeSequence
                .Skip(offset ?? 0).Take(count ?? 100)
                .AsViewModels<Event, EventViewModel>();

            return limitedSequence.ToPoll();
        }

        public async Task<Event> AddAsync(EventViewModel viewModel)
        {
            var newEvent = new Event();

            await AssignProperties(newEvent, viewModel);

            context.Events.Add(newEvent);

            await context.SaveChangesAsync();

            return newEvent;
        }

        public async Task<Event> FindAsync(int id)
        {
            var @event = await context.Events.FindAsync(id);

            if (@event == null)
            {
                return null;
            }

            var entry = context.Entry(@event);

            await context.LoadNavigationsAsync(entry, recursionDepth: 1);

            return @event;
        }

        public async Task<Event> UpdateAsync(int id, EventViewModel viewModel)
        {
            var eventToUpdate = context.Events.Find(id);

            if (eventToUpdate == null)
            {
                return null;
            }

            await AssignProperties(eventToUpdate, viewModel);

            await context.SaveChangesAsync();

            return eventToUpdate;
        }

        public async Task<Event> DeleteAsync(int id)
        {
            var eventToRemove = context.Events.Find(id);

            if (eventToRemove == null)
            {
                return null;
            }

            context.Events.Remove(eventToRemove);

            await context.SaveChangesAsync();

            return eventToRemove;
        }

        private async Task AssignProperties(Event @event, EventViewModel viewModel)
        {
            //если место с таким ID не найдено, будет брошено исключение
            var place = await context.Places.FirstAsync(existingPlace => existingPlace.Id == viewModel.Place.Id);

            @event.Title = viewModel.Title;
            @event.ShortTitle = viewModel.ShortTitle;
            @event.Description = viewModel.Description;
            @event.Dates = new List<EventDate>(viewModel.Dates);
            @event.Images = new List<ImageModel>(viewModel.Images);

            @event.Place = place;

            @event.EventCategories = viewModel.Categories.Select(categoryName => new EventCategory
            {
                //если такой категории ещё не было, она будет добавлена при сохранении; аналогично с тегом
                Category = FindCategoryByName(categoryName) ?? new Category { Name = categoryName },
                Event = @event
            }).ToHashSet();

            @event.EventTags = viewModel.Tags.Select(tagName => new EventTag
            {
                Tag = FindTagByName(tagName) ?? new Tag { Name = tagName },
                Event = @event
            }).ToHashSet();
        }

        private Category FindCategoryByName(string desiredName)
        {
            return context.Categories.FirstOrDefault(cat => cat.Name == desiredName);
        }

        private Tag FindTagByName(string desiredName)
        {
            return context.Tags.FirstOrDefault(tag => tag.Name == desiredName);
        }
    }
}