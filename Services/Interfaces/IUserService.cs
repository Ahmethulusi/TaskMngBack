using TaskMngBack.DTOs.Users;

namespace TaskMngBack.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto dto);
        Task DeleteAsync(int id);
    }
}
