using TaskManager_Staj_Project.DTOs.Auth;

namespace TaskManager_Staj_Project.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
