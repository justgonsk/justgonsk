using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JustGoModels.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<EventCategory> EventCategories { get; set; }
    }

    /// <summary>
    /// Связующая таблица для реализации отношения "many-to-many" между сущностями "<see cref="Models.Event"/> и "<see cref="Models.Category"/>"
    /// </summary>
    public class EventCategory
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }

        public virtual Event Event { get; set; }
        public virtual Category Category { get; set; }
    }
}