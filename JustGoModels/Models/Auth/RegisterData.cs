using System.ComponentModel.DataAnnotations;

namespace JustGoModels.Models.Auth
{
    public class RegisterData
    {
        [Required] public string Name { get; set; }
        [Required, EmailAddress] public string Email { get; set; }

        [Required, MinLength(5)] public string Password { get; set; }
    }
}