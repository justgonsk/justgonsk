using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JustGo.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<EventCategory> EventCategories { get; set; }
    }

    /// <summary>
    /// Связующая таблица для реализации отношения "many-to-many" между сущностями "<see cref="Models.Event"/> и "<see cref="Models.Category"/>"
    /// </summary>
    public class EventCategory
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }

        public Event Event { get; set; }
        public Category Category { get; set; }
    }
}