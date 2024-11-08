using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessAPI.Dtos.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(42), MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords does not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        //These 3 is optional.
        [PersonalData]
        [Range(1, 99)]
        public int? Age { get; set; }
        [PersonalData]
        [Range(1, 250)]
        public double? Height { get; set; } //In Cm
        [PersonalData]
        [Range(1, 250)]
        public double? Weight { get; set; } //In Kg
    }
}
