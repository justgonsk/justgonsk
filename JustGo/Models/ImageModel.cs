using System;
using System.ComponentModel.DataAnnotations;

namespace JustGo.Models
{
    public class ImageModel
    {
        [Url]
        public string Image { get; set; }
    }
}