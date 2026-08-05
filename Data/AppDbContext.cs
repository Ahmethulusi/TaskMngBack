using Microsoft.EntityFrameworkCore;
using TaskManager_Staj_Project.Models;
using TaskManager_Staj_Project.Models.Enums;

namespace TaskManager_Staj_Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

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
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "Sistem Yöneticisi",
                Email = "admin@taskmanager.local",
                // PASSWORD_HASH_PLACEHOLDER — ben dolduracağım ("Admin123!" şifresinin BCrypt hash'i)
                PasswordHash = "$2a$12$SaEu4o5t1yVB8Ko3jiSPd.vqf71KmA9Ctan2JHy2mVL4jQp/xHYwm",
                Role = UserRole.Admin,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
