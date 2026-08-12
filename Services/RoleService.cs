using TaskMngBack.DTOs.Roles;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RoleService(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<List<RoleDto>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(MapToDto).ToList();
        }

        public async Task<List<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            return permissions.Select(MapPermissionToDto).ToList();
        }

        public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
        {
            var existingRole = await _roleRepository.GetByNameAsync(dto.Name);
            if (existingRole != null)
            {
                throw new ConflictException($"'{dto.Name}' isimli rol zaten mevcut.");
            }

            var permissions = await ValidateAndGetPermissionsAsync(dto.PermissionIds);

            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Permissions = permissions
            };

            await _roleRepository.AddAsync(role);

            var created = await _roleRepository.GetByIdAsync(role.Id);
            return MapToDto(created!);
        }

        public async Task<RoleDto> UpdateAsync(Guid id, UpdateRoleDto dto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role is null)
            {
                throw new NotFoundException($"Id'si {id} olan rol bulunamadı.");
            }

            role.Name = dto.Name;

            var permissions = await ValidateAndGetPermissionsAsync(dto.PermissionIds);
            role.Permissions.Clear();
            foreach (var permission in permissions)
            {
                role.Permissions.Add(permission);
            }

            await _roleRepository.UpdateAsync(role);

            var updated = await _roleRepository.GetByIdAsync(id);
            return MapToDto(updated!);
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role is null)
            {
                throw new NotFoundException($"Id'si {id} olan rol bulunamadı.");
            }

            await _roleRepository.DeleteAsync(role);
        }

        private async Task<List<Permission>> ValidateAndGetPermissionsAsync(List<Guid> permissionIds)
        {
            if (permissionIds.Count == 0)
            {
                return new List<Permission>();
            }

            var distinctIds = permissionIds.Distinct().ToList();
            var permissions = await _permissionRepository.GetByIdsAsync(distinctIds);

            if (permissions.Count != distinctIds.Count)
            {
                throw new NotFoundException("Belirtilen izinlerden biri veya birkaçı bulunamadı.");
            }

            return permissions;
        }

        private static RoleDto MapToDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role.Permissions.Select(MapPermissionToDto).ToList()
            };
        }

        private static PermissionDto MapPermissionToDto(Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Key = permission.Key,
                Description = permission.Description
            };
        }
    }
}
