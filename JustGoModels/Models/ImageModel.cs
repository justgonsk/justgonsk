using System.ComponentModel.DataAnnotations;

namespace JustGoModels.Models
{
    public class ImageModel
    {
        [Url]
        public string Image { get; set; }
    }
}