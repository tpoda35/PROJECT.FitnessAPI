using FitnessAPI.Dtos.Auth;
using FitnessAPI.Helpers;
using FitnessAPI.Models;
using FitnessAPI.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitnessAPI.Services
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string? token)> LoginUser(LoginDto loginDto);
        Task<bool> RegisterUser(RegisterDto registerDto);
        Task<string?> GenerateTokenString(string Email);
        ClaimsPrincipal GetClaimsPrincipal(string? token);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IAuthRepository _authRepository;

        public AuthService(UserManager<AppUser> userManager, 
            IConfiguration configuration, ILogger<AuthService> logger,
            IAuthRepository authRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _authRepository = authRepository;
        }

        public async Task<string?> GenerateTokenString(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null) return null;

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, Email),
                new Claim(ClaimTypes.Role, "User")
                //The name is the same as the email.
            };

            var jwtKey = _configuration.GetSection("Jwt:Key").Value ??
                throw new ArgumentException("Jwt key is not configured.");
            var Issuer = _configuration.GetSection("Jwt:Issuer").Value ??
                throw new ArgumentException("Jwt issuer is not configured.");
            var Audience = _configuration.GetSection("Jwt:Audience").Value ??
                throw new ArgumentException("Jwt audience is not configured.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var signInCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                issuer: Issuer,
                audience: Audience,
                signingCredentials: signInCred);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public ClaimsPrincipal GetClaimsPrincipal(string? token)
        {
            var validation = new TokenValidationParameters()
            {
                ValidateActor = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidIssuer = _configuration.GetSection("Jwt:Issuer").Value,
                ValidAudience = _configuration.GetSection("Jwt:Audience").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration.GetSection("Jwt:Key").Value)),
                ClockSkew = TimeSpan.Zero
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }

        public async Task<(bool IsSuccess, string? token)> LoginUser(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) return (false, null);

            var loginResult = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!loginResult) return (false, null);

            string? refreshToken = user.RefreshToken;
            if (user.RefreshTokenExpiraton < DateTime.UtcNow || user.RefreshToken is null)
            {
                refreshToken = RefreshTokenHelper.GenerateRefreshToken();
                var result = await _authRepository.AddTokenToUser(user, refreshToken);
                if (!result) return (false, null);
            }

            return (loginResult, refreshToken);
        }

        public async Task<bool> RegisterUser(RegisterDto registerDto)
        {
            var newUser = new AppUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (result.Succeeded) return true;
            
            return false;
        }
    }
}
