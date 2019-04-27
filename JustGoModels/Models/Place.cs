using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JustGo.Interfaces;
using JustGo.View.Models;
using Newtonsoft.Json;

namespace JustGo.Models
{
    public class Place : IConvertibleToViewModel<PlaceViewModel>
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Address { get; set; }

        public virtual Coordinates Coordinates { get; set; }

        /// <summary>
        /// Навигационное свойство к событиям в этом месте
        /// </summary>
        public virtual ICollection<Event> Events { get; set; }

        public PlaceViewModel ToViewModel()
        {
            return new PlaceViewModel
            {
                Id = Id,
                Title = Title,
                Address = Address,
                Coordinates = new Coordinates
                {
                    Latitude = Coordinates.Latitude,
                    Longitude = Coordinates.Longitude
                }
            };
        }
    }
}