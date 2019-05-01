using System;

namespace JustGoModels.Models
{
    public class SingleDate
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public SingleDate(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }
}