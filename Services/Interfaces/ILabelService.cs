using TaskMngBack.DTOs.Labels;

namespace TaskMngBack.Services.Interfaces
{
    public interface ILabelService
    {
        Task<List<LabelDto>> GetAllAsync();
        Task<LabelDto> CreateAsync(CreateLabelDto dto);
        Task<LabelDto> UpdateAsync(Guid id, UpdateLabelDto dto);
        Task DeleteAsync(Guid id);
    }
}
