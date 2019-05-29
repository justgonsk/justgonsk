using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGoUtilities.Exceptions;
using JustGoUtilities.Data;
using JustGoModels.Interfaces;
using JustGoModels.Models;
using JustGoModels.Models.Edit;
using JustGoModels.Models.View;
using JustGoUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;

namespace JustGoUtilities.Repositories
{
    public class DbEventsRepository : IEventsRepository
    {
        private readonly MainContext context;

        public DbEventsRepository(MainContext context)
        {
            this.context = context;
        }

        public IEnumerable<Event> EnumerateAll() => context.Events;

        public Poll<EventViewModel> GetEventPoll(EventsFilter filter = null,
            int? offset = 0, int? count = 100)
        {
            var wholeSequence = context.Events
                .Include(e => e.EventCategories).ThenInclude(ec => ec.Category)
                .Include(e => e.EventTags).ThenInclude(et => et.Tag)
                .Include(e => e.Place).AsNoTracking();

            if (filter != null)
            {
                wholeSequence = filter.FilterEvents(wholeSequence).AsQueryable();
            }

            var limitedSequence = wholeSequence
                .Skip(offset ?? 0).Take(count ?? 100);

            var onlyModerated = limitedSequence.Where(e => e.IsModerated);

            return onlyModerated.AsEnumerable().ToPoll<Event, EventViewModel>();
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

        public async Task<Event> UpdateAsync(int id, EventEditModel editModel)
        {
            var eventToUpdate = context.Events.Find(id);

            if (eventToUpdate == null)
            {
                return null;
            }

            await UpdateProperties(eventToUpdate, editModel);

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

        public async Task AssignProperties(Event @event, EventViewModel viewModel)
        {
            var place = await context.Places.FirstOrDefaultAsync(existingPlace => existingPlace.Id == viewModel.Place.Id);
            @event.Title = viewModel.Title;
            @event.BodyText = viewModel.BodyText;
            @event.Source = viewModel.Source;
            @event.ShortTitle = viewModel.ShortTitle;
            @event.Description = viewModel.Description;
            @event.SingleDates = new List<SingleDate>(viewModel.SingleDates);
            @event.ScheduledDates = new List<ScheduledDate>(viewModel.ScheduledDates);
            @event.Images = new List<ImageModel>(viewModel.Images);

            @event.Place = place ?? throw new PlaceNotFoundException(viewModel.Place);

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

        public async Task UpdateProperties(Event @event, EventEditModel editModel)
        {
            if (editModel.Title != null)
            {
                @event.Title = editModel.Title;
            }

            if (editModel.IsModerated != null)
            {
                @event.IsModerated = editModel.IsModerated.Value;
            }

            if (editModel.ShortTitle != null)
            {
                @event.ShortTitle = editModel.ShortTitle;
            }

            //source shouldnt be updated
            /*if (editModel.Source != null)
            {
                @event.Source = editModel.Source;
            }*/

            if (editModel.Description != null)
            {
                @event.Description = editModel.Description;
            }

            if (editModel.SingleDates != null)
            {
                @event.SingleDates = new List<SingleDate>(editModel.SingleDates);
            }

            if (editModel.ScheduledDates != null)
            {
                @event.ScheduledDates = new List<ScheduledDate>(editModel.ScheduledDates);
            }

            if (editModel.Images != null)
            {
                @event.Images = new List<ImageModel>(editModel.Images);
            }

            if (editModel.Place != null)
            {
                @event.Place = await context.Places
                    .FirstAsync(existingPlace => existingPlace.Id == editModel.Place.Id);
            }

            if (editModel.Categories != null)
            {
                @event.EventCategories = editModel.Categories.Select(categoryName => new EventCategory
                {
                    //если такой категории ещё не было, она будет добавлена при сохранении; аналогично с тегом
                    Category = FindCategoryByName(categoryName) ?? new Category { Name = categoryName },
                    Event = @event
                }).ToHashSet();
            }

            if (editModel.Tags != null)
            {
                @event.EventTags = editModel.Tags.Select(tagName => new EventTag
                {
                    Tag = FindTagByName(tagName) ?? new Tag { Name = tagName },
                    Event = @event
                }).ToHashSet();
            }
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