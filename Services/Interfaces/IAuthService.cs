using TaskMngBack.DTOs.Auth;

namespace TaskMngBack.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}
