using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class UserPermissionResolver : IUserPermissionResolver
    {
        private readonly IRoleRepository _roleRepository;

        public UserPermissionResolver(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<string>> GetPermissionKeysAsync(int userId)
        {
            var roles = await _roleRepository.GetByUserIdAsync(userId);

            var permissions = roles
                .SelectMany(r => r.Permissions)
                .Select(p => p.Key)
                .Distinct()
                .ToList();

            return permissions;
        }
    }
}
