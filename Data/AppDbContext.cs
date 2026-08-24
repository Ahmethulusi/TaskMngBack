using Microsoft.EntityFrameworkCore;
using TaskMngBack.Models;
using TaskMngBack.Models.Enums;

namespace TaskMngBack.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
        public DbSet<Sprint> Sprints => Set<Sprint>();
        public DbSet<TaskStatusDefinition> TaskStatuses => Set<TaskStatusDefinition>();
        public DbSet<Label> Labels => Set<Label>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<CommentReaction> CommentReactions => Set<CommentReaction>();
        public DbSet<CommentMention> CommentMentions => Set<CommentMention>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasMany(t => t.AssignedUsers)
                .WithMany(u => u.AssignedTasks)
                .UsingEntity(j => j.ToTable("TaskAssignees"));

            modelBuilder.Entity<TaskItem>()
                .HasMany(t => t.Labels)
                .WithMany(l => l.Tasks)
                .UsingEntity(j => j.ToTable("TaskLabels"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.Departments)
                .WithMany(d => d.Users)
                .UsingEntity(j => j.ToTable("UserDepartments"));

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany()
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.StatusDefinition)
                .WithMany()
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.ParentTask)
                .WithMany(t => t.Subtasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Sprint>()
                .Property(s => s.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Sprint>()
                .HasOne(s => s.Project)
                .WithMany()
                .HasForeignKey(s => s.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Sprint)
                .WithMany(s => s.Tasks)
                .HasForeignKey(t => t.SprintId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TaskDependency>()
                .HasOne(d => d.Task)
                .WithMany(t => t.Dependencies)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskDependency>()
                .HasOne(d => d.DependsOnTask)
                .WithMany(t => t.Blocking)
                .HasForeignKey(d => d.DependsOnTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskDependency>()
                .HasIndex(d => new { d.TaskId, d.DependsOnTaskId })
                .IsUnique();

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentReaction>()
                .Property(r => r.Emoji)
                .HasMaxLength(10);

            modelBuilder.Entity<CommentReaction>()
                .HasOne(r => r.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(r => r.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentReaction>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentReaction>()
                .HasIndex(r => new { r.CommentId, r.UserId, r.Emoji })
                .IsUnique();

            modelBuilder.Entity<CommentMention>()
                .HasOne(m => m.Comment)
                .WithMany(c => c.Mentions)
                .HasForeignKey(m => m.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentMention>()
                .HasOne(m => m.MentionedUser)
                .WithMany()
                .HasForeignKey(m => m.MentionedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Task)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Comment)
                .WithMany(c => c.Attachments)
                .HasForeignKey(a => a.CommentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.UploadedByUser)
                .WithMany()
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ActivityLog>()
                .HasOne(a => a.Task)
                .WithMany(t => t.ActivityLogs)
                .HasForeignKey(a => a.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActivityLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .Property(n => n.Type)
                .HasMaxLength(50);
            modelBuilder.Entity<Notification>()
                .Property(n => n.Title)
                .HasMaxLength(200);
            modelBuilder.Entity<Notification>()
                .Property(n => n.Message)
                .HasMaxLength(500);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedTask)
                .WithMany()
                .HasForeignKey(n => n.RelatedTaskId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Key)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermissions",
                    j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionsId"),
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RolesId"),
                    j => j.HasKey("RolesId", "PermissionsId"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoles",
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RolesId"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UsersId"),
                    j => j.HasKey("UsersId", "RolesId"));

            modelBuilder.Entity<TaskStatusDefinition>().HasData(
                new TaskStatusDefinition
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Bekliyor",
                    DisplayOrder = 1,
                    ColorKey = "yellow",
                    IsDefault = true
                },
                new TaskStatusDefinition
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Devam Ediyor",
                    DisplayOrder = 2,
                    ColorKey = "orange",
                    IsDefault = false
                },
                new TaskStatusDefinition
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Tamamlandı",
                    DisplayOrder = 3,
                    ColorKey = "green",
                    IsDefault = false,
                    IsCompletionStatus = true
                }
            );

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "Sistem Yöneticisi",
                Email = "admin@taskmanager.local",
                // PASSWORD_HASH_PLACEHOLDER — ben dolduracağım ("Admin1234!" şifresinin BCrypt hash'i)
                PasswordHash = "$2a$12$t8fu4r4T2x5yE5xJbZjYEOqmJxiAz7iiGMTtBv4I3Fc0Az4fZwMsu",
                Role = UserRole.Admin,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            var adminRoleId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var userRoleId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId, Name = "Admin" },
                new Role { Id = userRoleId, Name = "User" }
            );

            var permDepartmentsManage = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var permUsersManage = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var permProjectsManage = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var permStatusesManage = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var permTasksViewAll = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var permTasksUpdateAll = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var permTasksDeleteAll = Guid.Parse("77777777-7777-7777-7777-777777777777");
            var permTasksAssign = Guid.Parse("88888888-8888-8888-8888-888888888888");
            var permRolesManage = Guid.Parse("99999999-9999-9999-9999-999999999999");

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = permDepartmentsManage, Key = "departments.manage", Description = "Departman oluşturma/düzenleme/silme" },
                new Permission { Id = permUsersManage, Key = "users.manage", Description = "Kullanıcı listeleme/düzenleme/silme" },
                new Permission { Id = permProjectsManage, Key = "projects.manage", Description = "Proje oluşturma/düzenleme/silme" },
                new Permission { Id = permStatusesManage, Key = "statuses.manage", Description = "Görev durumu tanımlarını yönetme" },
                new Permission { Id = permTasksViewAll, Key = "tasks.view.all", Description = "Tüm görevleri görüntüleme (sahiplik farketmeksizin)" },
                new Permission { Id = permTasksUpdateAll, Key = "tasks.update.all", Description = "Tüm görevleri güncelleme" },
                new Permission { Id = permTasksDeleteAll, Key = "tasks.delete.all", Description = "Tüm görevleri silme" },
                new Permission { Id = permTasksAssign, Key = "tasks.assign", Description = "Görevlere kullanıcı atama" },
                new Permission { Id = permRolesManage, Key = "roles.manage", Description = "Rol ve izin tanımlarını yönetme" }
            );

            modelBuilder.Entity("RolePermissions").HasData(
                new { RolesId = adminRoleId, PermissionsId = permDepartmentsManage },
                new { RolesId = adminRoleId, PermissionsId = permUsersManage },
                new { RolesId = adminRoleId, PermissionsId = permProjectsManage },
                new { RolesId = adminRoleId, PermissionsId = permStatusesManage },
                new { RolesId = adminRoleId, PermissionsId = permTasksViewAll },
                new { RolesId = adminRoleId, PermissionsId = permTasksUpdateAll },
                new { RolesId = adminRoleId, PermissionsId = permTasksDeleteAll },
                new { RolesId = adminRoleId, PermissionsId = permTasksAssign },
                new { RolesId = adminRoleId, PermissionsId = permRolesManage }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
