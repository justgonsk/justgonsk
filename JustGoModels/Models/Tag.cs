using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JustGoModels.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<EventTag> EventTags { get; set; }
    }

    /// <summary>
    /// Связующая таблица для реализации отношения "many-to-many" между сущностями "<see cref="Models.Event"/> и "<see cref="Models.Tag"/>"
    /// </summary>
    public class EventTag
    {
        public int EventId { get; set; }
        public int TagId { get; set; }

        public virtual Event Event { get; set; }
        public virtual Tag Tag { get; set; }
    }
}