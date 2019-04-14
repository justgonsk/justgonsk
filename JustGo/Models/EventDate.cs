using System;
namespace JustGo.Models
{
    public class EventDate
    {
        public DateTime? Start_Date { get; set; }
        public DateTime? Start_Time { get; set; }
        public DateTime? End_Date { get; set; }
        public DateTime? End_Time { get; set; }

        public bool Is_Continuous { get; set; }
        public bool Is_Endless { get; set; }
        public bool Is_Startless { get; set; }
    }
}
