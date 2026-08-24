using Microsoft.Extensions.Options;
using TaskMngBack.Configuration;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.BackgroundServices
{
    public class DeadlineNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly NotificationSettings _settings;
        private readonly ILogger<DeadlineNotificationService> _logger;

        public DeadlineNotificationService(
            IServiceScopeFactory scopeFactory,
            IOptions<NotificationSettings> settings,
            ILogger<DeadlineNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckDeadlinesAsync();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Deadline kontrolü başarısız oldu");
                }

                var intervalMinutes = _settings.CheckIntervalMinutes > 0
                    ? _settings.CheckIntervalMinutes
                    : 15;

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private async Task CheckDeadlinesAsync()
        {
            _logger.LogInformation("Deadline kontrolü başladı: {Time}", DateTime.UtcNow);

            using var scope = _scopeFactory.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

            var now = DateTime.UtcNow;
            var tasksDueSoon = await taskRepository.GetTasksDueSoonAsync(now, now.AddHours(24));

            foreach (var task in tasksDueSoon)
            {
                var recipients = task.AssignedUsers.Any()
                    ? task.AssignedUsers
                    : new List<User> { task.CreatedByUser };

                foreach (var user in recipients)
                {
                    var alreadyNotified = await notificationRepository
                        .ExistsAsync(user.Id, task.Id, "DeadlineApproaching");
                    if (alreadyNotified)
                    {
                        continue;
                    }

                    await notificationService.NotifyAsync(
                        user.Id,
                        "DeadlineApproaching",
                        "Yaklaşan teslim tarihi",
                        $"'{task.Title}' görevinin teslim tarihi yaklaşıyor.",
                        task.Id);
                }
            }

            _logger.LogInformation("Deadline kontrolü bitti, {Count} görev tarandı", tasksDueSoon.Count);
        }
    }
}
