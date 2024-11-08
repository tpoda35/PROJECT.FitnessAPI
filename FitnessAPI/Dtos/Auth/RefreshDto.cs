using System.ComponentModel.DataAnnotations;

namespace FitnessAPI.Dtos.Auth
{
    public class RefreshDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
