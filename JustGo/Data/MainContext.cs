using System;
using System.Linq;
using System.Reflection;
using JustGo.Helpers;
using JustGo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JustGo.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Главный контекст, служит для построения базы данных с помощью миграций.
    /// По этой причине должен содержать ссылки на все DbSet'ы, по которым
    /// будут строиться таблицы
    /// </summary>

    public class MainContext : DbContext
    {
        public DbSet<Event> Events { set; get; }
        public DbSet<Place> Places { set; get; }
        public DbSet<Category> Categories { set; get; }
        public DbSet<Tag> Tags { set; get; }

        public DbSet<EventCategory> EventCategories { set; get; }
        public DbSet<EventTag> EventTags { set; get; }
        public DbSet<EventDate> EventDates { set; get; }

        public DbSet<EventsKeyMapping> EventsKeyMappings { set; get; }
        public DbSet<PlacesKeyMapping> PlacesKeyMappings { set; get; }

        public MainContext(DbContextOptions<MainContext> options)
            : base(options)
        {
            //раскомментировать, если нужно продебажить код создания файлов миграций, исполняемый самой студией
            //Debugger.Launch();

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().Property(t => t.Images)
                .HasConversion(DbUtilities.ImagesLinksConverter);

            modelBuilder.Entity<Coordinates>(IgnoreBaseProperties);

            modelBuilder.Entity<Place>().OwnsOne(place => place.Coordinates);

            modelBuilder.Entity<EventCategory>()
                .HasKey(ec => new { ec.EventId, ec.CategoryId });

            modelBuilder.Entity<EventTag>()
                .HasKey(et => new { et.EventId, et.TagId });
        }

        private void IgnoreBaseProperties<T>(EntityTypeBuilder<T> builder) where T : class
        {
            var baseProperties = typeof(T).BaseType.GetProperties().Select(p => p.Name);
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                .Select(p => p.Name);

            foreach (var baseProperty in baseProperties.Except(properties))
            {
                builder.Ignore(baseProperty);
            }
        }
    }
}