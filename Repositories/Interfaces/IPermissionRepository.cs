using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<List<Permission>> GetAllAsync();
        Task<List<Permission>> GetByIdsAsync(List<Guid> ids);
    }
}
