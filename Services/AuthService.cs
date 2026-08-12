using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskMngBack.Configuration;
using TaskMngBack.DTOs.Auth;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Models.Enums;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserPermissionResolver _permissionResolver;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserPermissionResolver permissionResolver,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionResolver = permissionResolver;
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

            var userRole = await _roleRepository.GetByNameAsync("User");
            if (userRole != null)
            {
                user.Roles.Add(userRole);
                await _userRepository.UpdateAsync(user);
            }

            var userWithRoles = await _userRepository.GetByIdAsync(user.Id);
            return await BuildAuthResponseAsync(userWithRoles!);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new InvalidOperationException("Email veya şifre hatalı.");
            }

            return await BuildAuthResponseAsync(user);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException($"Id'si {userId} olan kullanıcı bulunamadı.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                throw new BadRequestException("Mevcut şifre yanlış.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.MustChangePassword = false;

            await _userRepository.UpdateAsync(user);
        }

        private async Task<AuthResponseDto> BuildAuthResponseAsync(User user)
        {
            var permissions = await _permissionResolver.GetPermissionKeysAsync(user.Id);

            return new AuthResponseDto
            {
                Token = await GenerateTokenAsync(user),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Permissions = permissions,
                MustChangePassword = user.MustChangePassword
            };
        }

        private async Task<string> GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var permissions = await _permissionResolver.GetPermissionKeysAsync(user.Id);
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

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
