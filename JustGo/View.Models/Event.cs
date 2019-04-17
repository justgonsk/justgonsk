using System;
using System.Collections.Generic;
using JustGo.Models;

namespace JustGo.View.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body_Text { get; set; } // полное описание
        public Place Place { get; set; } // место проведения
        public List<string> Categories { get; set; } // список категорий
        public List<string> Tags { get; set; }
        public List<Image_Model> Images { get; set; }
        public List<EventDate> Dates { get; set; }

        public bool HasCategories(List<string> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            foreach (var category in categories)
            {
                if (!this.Categories.Contains(category))
                {
                    return false; // если нет хотя бы одной категории
                }
            }

            return true;
        }
    }
}
