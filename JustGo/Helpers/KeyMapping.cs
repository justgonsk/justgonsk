using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JustGo.Models;

namespace JustGo.Helpers
{
    /// <summary>
    /// Отображает ключи событий kudago на ключи в нашей базе
    /// </summary>
    public class EventsKeyMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KudagoId { get; set; }

        public int OurId { get; set; }

        [ForeignKey(nameof(OurId))]
        public Event Event { get; set; }
    }

    /// <summary>
    /// Отображает ключи мест kudago на ключи в нашей базе
    /// </summary>
    public class PlacesKeyMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KudagoId { get; set; }

        public int OurId { get; set; }

        [ForeignKey(nameof(OurId))]
        public Place Place { get; set; }
    }
}