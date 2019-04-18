using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JustGo.Helpers;
using JustGo.Models;
using Microsoft.EntityFrameworkCore;

namespace JustGo.Contexts
{
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

        public DbSet<EventsKeyMapping> EventsKeyMappings { set; get; }
        public DbSet<PlacesKeyMapping> PlacesKeyMappings { set; get; }

        public MainContext(DbContextOptions<MainContext> options)
            : base(options)
        {
            //раскомментировать, если нужно продебажить код создания файлов миграций, исполняемый самой студией
            //Debugger.Launch();
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

            modelBuilder.Entity<EventCategory>().HasKey(ec => new { ec.EventId, ec.CategoryId });
            modelBuilder.Entity<EventTag>().HasKey(et => new { et.EventId, et.TagId });
        }
    }
}