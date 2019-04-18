using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JustGo.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<EventTag> EventTags { get; set; }
    }

    /// <summary>
    /// Связующая таблица для реализации отношения "many-to-many" между сущностями "<see cref="Models.Event"/> и "<see cref="Models.Tag"/>"
    /// </summary>
    public class EventTag
    {
        public int EventId { get; set; }
        public int TagId { get; set; }

        public Event Event { get; set; }
        public Tag Tag { get; set; }
    }
}