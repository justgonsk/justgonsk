using System.ComponentModel.DataAnnotations;

namespace JustGoModels.Models.Auth
{
    public class LoginData
    {
        [Required] public string Name { get; set; }
        [Required, MinLength(5)] public string Password { get; set; }
    }
}