using TaskMngBack.DTOs.Roles;

namespace TaskMngBack.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllAsync();
        Task<List<PermissionDto>> GetAllPermissionsAsync();
        Task<RoleDto> CreateAsync(CreateRoleDto dto);
        Task<RoleDto> UpdateAsync(Guid id, UpdateRoleDto dto);
        Task DeleteAsync(Guid id);
    }
}
