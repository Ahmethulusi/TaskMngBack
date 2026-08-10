using TaskMngBack.DTOs.Statuses;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class TaskStatusService : ITaskStatusService
    {
        private readonly ITaskStatusRepository _taskStatusRepository;
        private readonly ITaskRepository _taskRepository;
        private static readonly HashSet<string> ValidColorKeys = new() { "gray", "yellow", "orange", "green", "blue", "purple" };

        public TaskStatusService(
            ITaskStatusRepository taskStatusRepository,
            ITaskRepository taskRepository)
        {
            _taskStatusRepository = taskStatusRepository;
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskStatusDto>> GetAllAsync()
        {
            var statuses = await _taskStatusRepository.GetAllAsync();
            return statuses.Select(MapToDto).ToList();
        }

        public async Task<TaskStatusDto> CreateAsync(CreateTaskStatusDto dto)
        {
            ValidateColorKey(dto.ColorKey);

            if (dto.IsDefault)
            {
                await ClearExistingDefaultAsync();
            }

            var maxDisplayOrder = (await _taskStatusRepository.GetAllAsync())
                .DefaultIfEmpty()
                .Max(s => s?.DisplayOrder ?? 0);

            var status = new TaskStatusDefinition
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ColorKey = dto.ColorKey,
                IsDefault = dto.IsDefault,
                DisplayOrder = maxDisplayOrder + 1
            };

            await _taskStatusRepository.AddAsync(status);

            var created = await GetStatusOrThrowAsync(status.Id);
            return MapToDto(created);
        }

        public async Task<TaskStatusDto> UpdateAsync(Guid id, UpdateTaskStatusDefinitionDto dto)
        {
            var status = await GetStatusOrThrowAsync(id);

            ValidateColorKey(dto.ColorKey);

            if (dto.IsDefault && !status.IsDefault)
            {
                await ClearExistingDefaultAsync();
            }

            status.Name = dto.Name;
            status.ColorKey = dto.ColorKey;
            status.DisplayOrder = dto.DisplayOrder;
            status.IsDefault = dto.IsDefault;

            await _taskStatusRepository.UpdateAsync(status);

            var updated = await GetStatusOrThrowAsync(id);
            return MapToDto(updated);
        }

        public async Task DeleteAsync(Guid id)
        {
            var status = await GetStatusOrThrowAsync(id);

            if (status.IsDefault)
            {
                throw new ConflictException("Varsayılan durum silinemez, önce başka bir durumu varsayılan yapın.");
            }

            if (await _taskRepository.HasTasksForStatusAsync(id))
            {
                throw new ConflictException("Bu duruma atanmış görevler var, önce onları başka bir duruma taşıyın.");
            }

            await _taskStatusRepository.DeleteAsync(status);
        }

        private static void ValidateColorKey(string colorKey)
        {
            if (!ValidColorKeys.Contains(colorKey))
            {
                throw new BadRequestException($"Geçersiz renk anahtarı. İzin verilen değerler: {string.Join(", ", ValidColorKeys)}");
            }
        }

        private async Task ClearExistingDefaultAsync()
        {
            var existingDefault = await _taskStatusRepository.GetDefaultAsync();
            if (existingDefault is not null)
            {
                existingDefault.IsDefault = false;
                await _taskStatusRepository.UpdateAsync(existingDefault);
            }
        }

        private async Task<TaskStatusDefinition> GetStatusOrThrowAsync(Guid id)
        {
            var status = await _taskStatusRepository.GetByIdAsync(id);

            if (status is null)
            {
                throw new NotFoundException($"Id'si {id} olan durum bulunamadı.");
            }

            return status;
        }

        private static TaskStatusDto MapToDto(TaskStatusDefinition status)
        {
            return new TaskStatusDto
            {
                Id = status.Id,
                Name = status.Name,
                DisplayOrder = status.DisplayOrder,
                ColorKey = status.ColorKey,
                IsDefault = status.IsDefault
            };
        }
    }
}
