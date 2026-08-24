using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<TaskItem>> GetAllAsync()
        {
            return IncludeNavigations(_context.Tasks).ToListAsync();
        }

        public Task<List<TaskItem>> GetByUserAsync(int userId)
        {
            return IncludeNavigations(_context.Tasks)
                .Where(t => t.CreatedByUserId == userId || 
                           t.AssignedUsers.Any(u => u.Id == userId) ||
                           (t.ProjectId != null && t.Project.Members.Any(m => m.UserId == userId)) ||
                           (t.DepartmentId != null && t.Department.Users.Any(u => u.Id == userId)))
                .ToListAsync();
        }

        public Task<List<int>> GetTaskIdsByProjectAsync(Guid projectId)
        {
            return _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => t.Id)
                .ToListAsync();
        }

        public Task<TaskItem?> GetByIdAsync(int id)
        {
            return IncludeNavigations(_context.Tasks).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(TaskDependency dependency)
        {
            await _context.TaskDependencies.AddAsync(dependency);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskDependency dependency)
        {
            _context.TaskDependencies.Remove(dependency);
            await _context.SaveChangesAsync();
        }

        public Task<TaskDependency?> GetDependencyAsync(int taskId, int dependsOnTaskId)
        {
            return _context.TaskDependencies.FirstOrDefaultAsync(d =>
                d.TaskId == taskId && d.DependsOnTaskId == dependsOnTaskId);
        }

        public async Task<bool> WouldCreateCycleAsync(int taskId, int dependsOnTaskId)
        {
            var edges = await _context.TaskDependencies
                .AsNoTracking()
                .Select(d => new { d.TaskId, d.DependsOnTaskId })
                .ToListAsync();

            var dependenciesByTask = edges
                .GroupBy(d => d.TaskId)
                .ToDictionary(g => g.Key, g => g.Select(d => d.DependsOnTaskId).ToList());

            var pending = new Stack<int>();
            var visited = new HashSet<int>();
            pending.Push(dependsOnTaskId);

            while (pending.Count > 0)
            {
                var currentTaskId = pending.Pop();

                if (currentTaskId == taskId)
                {
                    return true;
                }

                if (!visited.Add(currentTaskId) ||
                    !dependenciesByTask.TryGetValue(currentTaskId, out var dependencies))
                {
                    continue;
                }

                foreach (var dependencyId in dependencies)
                {
                    pending.Push(dependencyId);
                }
            }

            return false;
        }

        public Task<bool> HasTasksForUserAsync(int userId)
        {
            return _context.Tasks.AnyAsync(t =>
                t.CreatedByUserId == userId ||
                t.AssignedUsers.Any(u => u.Id == userId));
        }

        public Task<bool> HasTasksForStatusAsync(Guid statusId)
        {
            return _context.Tasks.AnyAsync(t => t.StatusId == statusId);
        }

        public Task<int> GetCommentCountAsync(int taskId)
        {
            return _context.Comments.CountAsync(c => c.TaskId == taskId);
        }

        public Task<List<TaskItem>> GetTasksDueSoonAsync(DateTime fromUtc, DateTime toUtc)
        {
            return _context.Tasks
                .Include(t => t.AssignedUsers)
                .Include(t => t.CreatedByUser)
                .Include(t => t.StatusDefinition)
                .Where(t => t.DueDate.HasValue
                    && t.DueDate >= fromUtc
                    && t.DueDate <= toUtc
                    && !t.StatusDefinition.IsCompletionStatus)
                .ToListAsync();
        }

        private static IQueryable<TaskItem> IncludeNavigations(IQueryable<TaskItem> query)
        {
            return query
                .Include(t => t.StatusDefinition)
                .Include(t => t.Department)
                    .ThenInclude(d => d.Users)
                .Include(t => t.Project)
                    .ThenInclude(p => p.Members)
                .Include(t => t.Sprint)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedUsers)
                .Include(t => t.Labels)
                .Include(t => t.Attachments)
                .Include(t => t.ParentTask)
                .Include(t => t.Subtasks)
                    .ThenInclude(s => s.StatusDefinition)
                .Include(t => t.Dependencies)
                    .ThenInclude(d => d.DependsOnTask)
                        .ThenInclude(t => t.StatusDefinition)
                .Include(t => t.Blocking)
                    .ThenInclude(d => d.Task)
                        .ThenInclude(t => t.StatusDefinition);
        }
    }
}
