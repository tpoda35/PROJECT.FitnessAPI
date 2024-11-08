using System.Security.Cryptography;

namespace FitnessAPI.Helpers
{
    public static class RefreshTokenHelper
    {
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            var generator = RandomNumberGenerator.Create();

            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}