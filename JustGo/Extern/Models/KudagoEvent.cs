using JustGo.View.Models;

namespace JustGo.Extern.Models
{
    public class KudagoEvent : EventViewModel
    {
        /// <summary>
        /// ID в базе Kudago.
        /// </summary>
        public int Id { get; set; }

        public new KudagoPlace Place { get; set; } // место проведения

    }
}