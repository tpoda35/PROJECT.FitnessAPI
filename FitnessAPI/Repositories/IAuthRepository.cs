using FitnessAPI.Data;
using FitnessAPI.Models;

namespace FitnessAPI.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> AddTokenToUser(AppUser user, string refreshToken);
    }

    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddTokenToUser(AppUser user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiraton = DateTime.UtcNow.AddMonths(1);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
