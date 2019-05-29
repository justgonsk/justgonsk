using System.Collections.Generic;
using JustGoModels.Models;
using JustGoModels.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JustGoUtilities.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Главный контекст, служит для построения базы данных с помощью миграций.
    /// По этой причине должен содержать ссылки на все DbSet'ы, по которым
    /// будут строиться таблицы
    /// </summary>

    public class MainContext : IdentityDbContext<JustGoUser>
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
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ForNpgsqlUseIdentityColumns();

            modelBuilder.Entity<Event>().Property(t => t.Images)
                .HasConversion(DbUtilities.ImagesLinksConverter);

            modelBuilder.Entity<Event>().Property(t => t.SingleDates)
                .HasConversion(DbUtilities.JsonConverter<List<SingleDate>>());

            modelBuilder.Entity<Event>().Property(t => t.ScheduledDates)
                .HasConversion(DbUtilities.JsonConverter<List<ScheduledDate>>());

            modelBuilder.Entity<Coordinates>(DbUtilities.IgnoreBaseProperties);

            modelBuilder.Entity<Place>().OwnsOne(place => place.Coordinates);

            modelBuilder.Entity<EventCategory>()
                .HasKey(ec => new { ec.EventId, ec.CategoryId });

            modelBuilder.Entity<EventTag>()
                .HasKey(et => new { et.EventId, et.TagId });

            modelBuilder.Entity<JustGoUser>(DbUtilities.SaveBoolAsBit);
            modelBuilder.Entity<JustGoUser>(DbUtilities.SaveStringAsVarchar100);

            modelBuilder.Entity<IdentityUserClaim<string>>(DbUtilities.SaveBoolAsBit);
            modelBuilder.Entity<IdentityUserClaim<string>>(DbUtilities.SaveStringAsVarchar100);

            modelBuilder.Entity<IdentityUserLogin<string>>(DbUtilities.SaveBoolAsBit);
            modelBuilder.Entity<IdentityUserLogin<string>>(DbUtilities.SaveStringAsVarchar100);

            modelBuilder.Entity<IdentityUserRole<string>>(DbUtilities.SaveBoolAsBit);
            modelBuilder.Entity<IdentityUserRole<string>>(DbUtilities.SaveStringAsVarchar100);

            modelBuilder.Entity<IdentityUserToken<string>>(DbUtilities.SaveBoolAsBit);
            modelBuilder.Entity<IdentityUserToken<string>>(DbUtilities.SaveStringAsVarchar100);
        }
    }
}