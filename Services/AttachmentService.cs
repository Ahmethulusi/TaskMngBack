using TaskMngBack.DTOs.Attachments;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IStorageService _storageService;

        public AttachmentService(
            IAttachmentRepository attachmentRepository,
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IDepartmentRepository departmentRepository,
            IStorageService storageService)
        {
            _attachmentRepository = attachmentRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _departmentRepository = departmentRepository;
            _storageService = storageService;
        }

        public async Task<PresignUploadResponseDto> PresignUpload(PresignUploadRequestDto dto, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(dto.TaskId);
            await EnsureCanViewTaskAsync(task, userId, permissions);

            if (dto.FileSizeBytes > 10 * 1024 * 1024)
            {
                throw new BadRequestException("Dosya boyutu 10MB'ı geçemez.");
            }

            var storageKey = $"tasks/{dto.TaskId}/{Guid.NewGuid()}-{SanitizeFileName(dto.FileName)}";
            var uploadUrl = await _storageService.GeneratePresignedUploadUrlAsync(storageKey, dto.ContentType);

            return new PresignUploadResponseDto
            {
                UploadUrl = uploadUrl,
                StorageKey = storageKey
            };
        }

        public async Task<AttachmentDto> Confirm(ConfirmAttachmentDto dto, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(dto.TaskId);
            await EnsureCanViewTaskAsync(task, userId, permissions);

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = dto.FileName,
                FileSize = dto.FileSize,
                ContentType = dto.ContentType,
                StorageKey = dto.StorageKey,
                TaskId = dto.TaskId,
                CommentId = null,
                UploadedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _attachmentRepository.AddAsync(attachment);

            var created = await _attachmentRepository.GetByIdAsync(attachment.Id)
                ?? throw new NotFoundException("Ek dosya kaydı oluşturulamadı.");

            return await MapToDtoAsync(created);
        }

        public async Task<List<AttachmentDto>> GetForTask(int taskId, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);
            await EnsureCanViewTaskAsync(task, userId, permissions);

            var attachments = await _attachmentRepository.GetByTaskIdAsync(taskId);
            var dtos = new List<AttachmentDto>(attachments.Count);

            foreach (var attachment in attachments)
            {
                dtos.Add(await MapToDtoAsync(attachment));
            }

            return dtos;
        }

        public async Task Delete(Guid attachmentId, int userId, List<string> permissions)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);

            if (attachment is null)
            {
                throw new NotFoundException($"Id'si {attachmentId} olan ek dosya bulunamadı.");
            }

            if (attachment.UploadedByUserId != userId && !permissions.Contains("tasks.delete.all"))
            {
                throw new ForbiddenAccessException("Bu ek dosyayı silme yetkiniz yok.");
            }

            await _storageService.DeleteObjectAsync(attachment.StorageKey);
            await _attachmentRepository.DeleteAsync(attachment);
        }

        private async Task<TaskItem> GetTaskOrThrowAsync(int taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task is null)
            {
                throw new NotFoundException($"Id'si {taskId} olan görev bulunamadı.");
            }

            return task;
        }

        private async Task EnsureCanViewTaskAsync(TaskItem task, int userId, List<string> permissions)
        {
            if (permissions.Contains("tasks.view.all") ||
                task.CreatedByUserId == userId ||
                task.AssignedUsers.Any(u => u.Id == userId) ||
                await IsProjectMemberAsync(task, userId) ||
                await IsDepartmentMemberAsync(task, userId) ||
                await IsDepartmentManagerAsync(task, userId))
            {
                return;
            }

            throw new ForbiddenAccessException("Bu görevin ek dosyalarına erişim yetkiniz yok.");
        }

        private async Task<bool> IsProjectMemberAsync(TaskItem task, int userId)
        {
            var role = await _projectRepository.GetMemberRoleAsync(task.ProjectId, userId);
            return role != null;
        }

        private async Task<bool> IsDepartmentManagerAsync(TaskItem task, int userId)
        {
            if (task.DepartmentId == null) return false;
            var department = await _departmentRepository.GetByIdAsync(task.DepartmentId.Value);
            return department?.ManagerId == userId;
        }

        private async Task<bool> IsDepartmentMemberAsync(TaskItem task, int userId)
        {
            if (task.DepartmentId == null) return false;
            var department = await _departmentRepository.GetByIdAsync(task.DepartmentId.Value);
            return department?.Users.Any(u => u.Id == userId) ?? false;
        }

        private async Task<AttachmentDto> MapToDtoAsync(Attachment attachment)
        {
            return new AttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                FileSize = attachment.FileSize,
                ContentType = attachment.ContentType,
                UploadedByUserId = attachment.UploadedByUserId,
                UploadedByUserName = attachment.UploadedByUser?.FullName ?? string.Empty,
                CreatedAt = attachment.CreatedAt,
                DownloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(attachment.StorageKey)
            };
        }

        private static string SanitizeFileName(string fileName)
        {
            var sanitized = new string(fileName
                .Replace(' ', '-')
                .Where(c => char.IsLetterOrDigit(c) || c == '.' || c == '-')
                .ToArray());

            return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
        }
    }
}
