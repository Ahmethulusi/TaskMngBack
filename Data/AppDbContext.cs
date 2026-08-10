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
        public DbSet<TaskStatusDefinition> TaskStatuses => Set<TaskStatusDefinition>();
        public DbSet<Label> Labels => Set<Label>();

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
                    IsDefault = false
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
