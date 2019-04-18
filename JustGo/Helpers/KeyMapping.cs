using JustGo.Models;

namespace JustGo.Helpers
{
    /// <summary>
    /// Отображает ключи событий kudago на ключи в нашей базе
    /// </summary>
    public class EventsKeyMapping
    {
        public int KudagoId { get; set; }
        public int OurId { get; set; }

        public Event Event { get; set; }
    }

    /// <summary>
    /// Отображает ключи мест kudago на ключи в нашей базе
    /// </summary>
    public class PlacesKeyMapping
    {
        public int KudagoId { get; set; }
        public int OurId { get; set; }

        public Place Place { get; set; }
    }
}