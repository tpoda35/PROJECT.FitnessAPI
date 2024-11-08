using FitnessAPI.Dtos.Auth;
using FitnessAPI.Models;
using FitnessAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, 
            ILogger<AuthController> logger, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            var results = await _authService.LoginUser(loginDto);

            if (results.IsSuccess)
            {
                var JwtToken = await _authService.GenerateTokenString(loginDto.Email);
                if (JwtToken is null) return BadRequest("User not found.");

                return Ok(new LoginResponseDto()
                {
                    AccessToken = JwtToken,
                    AccessTokenExpiration = DateTime.UtcNow.AddMinutes(20),
                    RefreshToken = results.token,
                    RefreshTokenExpiration = DateTime.UtcNow.AddMonths(1)
                });
            }

            return BadRequest("Something went wrong during login.");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
        {
            if (await _authService.RegisterUser(registerDto))
            {
                return Ok("User successfully registered.");
            }

            return BadRequest("Something went wrong during account creation.");
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(RefreshDto refreshDto)
        {
            var principal = _authService.GetClaimsPrincipal(refreshDto.AccessToken);

            if (principal?.Identity?.Name is null) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(principal.Identity.Name);
            if (user is null ||
                user.RefreshToken != refreshDto.RefreshToken ||
                user.RefreshTokenExpiraton < DateTime.UtcNow) return Unauthorized();

            var tokenString = await _authService.GenerateTokenString(principal.Identity.Name);
            if (tokenString is null) return BadRequest("User not found.");

            return Ok(new LoginResponseDto()
            {
                AccessToken = tokenString,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(20),
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiration = user.RefreshTokenExpiraton
            });
        }
    }
}
