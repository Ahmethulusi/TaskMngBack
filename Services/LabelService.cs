using TaskMngBack.DTOs.Labels;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class LabelService : ILabelService
    {
        private readonly ILabelRepository _labelRepository;

        public LabelService(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public async Task<List<LabelDto>> GetAllAsync()
        {
            var labels = await _labelRepository.GetAllAsync();
            return labels.Select(MapToDto).ToList();
        }

        public async Task<LabelDto> CreateAsync(CreateLabelDto dto)
        {
            var label = new Label
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };

            await _labelRepository.AddAsync(label);

            var created = await GetLabelOrThrowAsync(label.Id);
            return MapToDto(created);
        }

        public async Task<LabelDto> UpdateAsync(Guid id, UpdateLabelDto dto)
        {
            var label = await GetLabelOrThrowAsync(id);

            label.Name = dto.Name;

            await _labelRepository.UpdateAsync(label);

            var updated = await GetLabelOrThrowAsync(id);
            return MapToDto(updated);
        }

        public async Task DeleteAsync(Guid id)
        {
            var label = await GetLabelOrThrowAsync(id);
            await _labelRepository.DeleteAsync(label);
        }

        private async Task<Label> GetLabelOrThrowAsync(Guid id)
        {
            var label = await _labelRepository.GetByIdAsync(id);

            if (label is null)
            {
                throw new NotFoundException($"Id'si {id} olan etiket bulunamadı.");
            }

            return label;
        }

        private static LabelDto MapToDto(Label label)
        {
            return new LabelDto
            {
                Id = label.Id,
                Name = label.Name
            };
        }
    }
}
