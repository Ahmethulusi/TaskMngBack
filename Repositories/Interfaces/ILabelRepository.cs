using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface ILabelRepository
    {
        Task<List<Label>> GetAllAsync();
        Task<Label?> GetByIdAsync(Guid id);
        Task<List<Label>> GetByIdsAsync(List<Guid> ids);
        Task AddAsync(Label label);
        Task UpdateAsync(Label label);
        Task DeleteAsync(Label label);
    }
}
