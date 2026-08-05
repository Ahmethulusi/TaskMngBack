using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManager_Staj_Project.Configuration;
using TaskManager_Staj_Project.DTOs.Auth;
using TaskManager_Staj_Project.Models;
using TaskManager_Staj_Project.Models.Enums;
using TaskManager_Staj_Project.Repositories.Interfaces;
using TaskManager_Staj_Project.Services.Interfaces;

namespace TaskManager_Staj_Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                throw new InvalidOperationException("Bu email adresi zaten kayıtlı.");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new InvalidOperationException("Email veya şifre hatalı.");
            }

            return BuildAuthResponse(user);
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            return new AuthResponseDto
            {
                Token = GenerateToken(user),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
