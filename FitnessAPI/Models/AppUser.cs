using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessAPI.Models
{
    public class AppUser : IdentityUser
    {
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

        //RefreshToken
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiraton { get; set; }
    }
}
