using System;
namespace JustGo.Models
{
    public class Place
    {
        public long Id { get; set; }
        public string Short_Title { get; set; }
        public Coordinates Coords { get; set; }
    }
}
