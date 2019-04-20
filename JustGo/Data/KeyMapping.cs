using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JustGo.Models;

namespace JustGo.Data
{
    /// <summary>
    /// Отображает ключи событий kudago на ключи в нашей базе
    /// </summary>
    public class EventsKeyMapping
    {
        //2-ой атрибут указывает, что ключ генерериует не EF, а мы сами (точнее он приходит от kudago)
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KudagoId { get; set; }

        public int OurId { get; set; }

        [ForeignKey(nameof(OurId))]
        public virtual Event Event { get; set; }
    }

    /// <summary>
    /// Отображает ключи мест kudago на ключи в нашей базе
    /// </summary>
    public class PlacesKeyMapping
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KudagoId { get; set; }

        public int OurId { get; set; }

        [ForeignKey(nameof(OurId))]
        public virtual Place Place { get; set; }
    }
}