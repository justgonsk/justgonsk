using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Device.Location;
using JustGo.Helpers;
using JustGo.View.Models;
using Newtonsoft.Json;

namespace JustGo.Models
{
    public class Place : IConvertibleToViewModel<PlaceViewModel>
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Address { get; set; }

        public Coordinates Coordinates { get; set; }

        public ICollection<Event> Events { get; set; }

        public PlaceViewModel ConvertToViewModel()
        {
            return new PlaceViewModel
            {
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