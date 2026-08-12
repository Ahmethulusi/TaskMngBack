using TaskMngBack.DTOs.Users;

namespace TaskMngBack.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> GetOwnProfileAsync(int userId);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto dto);
        Task<UserDto> UpdateOwnProfileAsync(int userId, UpdateOwnProfileDto dto);
        Task DeleteAsync(int id);
    }
}
