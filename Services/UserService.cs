using TaskManager_Staj_Project.DTOs.Users;
using TaskManager_Staj_Project.Exceptions;
using TaskManager_Staj_Project.Models;
using TaskManager_Staj_Project.Models.Enums;
using TaskManager_Staj_Project.Repositories.Interfaces;
using TaskManager_Staj_Project.Services.Interfaces;

namespace TaskManager_Staj_Project.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await GetUserOrThrowAsync(id);
            return MapToDto(user);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await GetUserOrThrowAsync(id);

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Role = Enum.Parse<UserRole>(dto.Role, ignoreCase: true);

            await _userRepository.UpdateAsync(user);

            return MapToDto(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetUserOrThrowAsync(id);
            await _userRepository.DeleteAsync(user);
        }

        private async Task<User> GetUserOrThrowAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"Id'si {id} olan kullanıcı bulunamadı.");
            }

            return user;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }
    }
}
