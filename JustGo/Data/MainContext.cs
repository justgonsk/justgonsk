using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JustGo.Helpers;
using Microsoft.EntityFrameworkCore;
using JustGo.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JustGo.Contexts
{
    public class MainContext : DbContext
    {
        public DbSet<Event> Events { set; get; }
        public DbSet<Place> Places { set; get; }
        public DbSet<Category> Categories { set; get; }
        public DbSet<EventCategory> EventCategories { set; get; }
        public DbSet<EventTag> EventTags { set; get; }
        public DbSet<Tag> Tags { set; get; }

        public MainContext(DbContextOptions<MainContext> options)
            : base(options)
        {
            Debugger.Launch();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().Property(t => t.Images).HasConversion(DbConverters.ImagesConverter);

            modelBuilder.Entity<Coordinates>(builder =>
            {
                var baseProperties = typeof(Coordinates).BaseType.GetProperties().Select(p => p.Name);
                var properties = typeof(Coordinates)
                    .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                    .Select(p => p.Name);

                foreach (var baseProperty in baseProperties.Except(properties))
                {
                    builder.Ignore(baseProperty);
                }
            });

            modelBuilder.Entity<Place>().OwnsOne(place => place.Coordinates);

            modelBuilder.Entity<EventDate>().ToTable("EventDates").HasKey(date => new { date.EventId, date.Start });
        }
    }
}