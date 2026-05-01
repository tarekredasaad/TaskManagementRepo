using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyProject.Infrastructure.CashService;
using MyProject.Infrastructure.Repositories.Tasks;

namespace MyProject.Infrastructure.Services;

public class TaskBackgroundWorker : BackgroundService
{
    private readonly ITaskProcessingQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TaskBackgroundWorker> _logger;

    public TaskBackgroundWorker(
        ITaskProcessingQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<TaskBackgroundWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var taskId = await _queue.DequeueAsync(stoppingToken);

            using var scope = _scopeFactory.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cache = scope.ServiceProvider.GetRequiredService<RedisCacheService>();

            var taskItem = await taskRepository.GetByIdAsync(taskId);
            if (taskItem is null)
            {
                continue;
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            taskItem.Status = Domain.Enum.TaskStatus.InProgress;
            taskRepository.Update(taskItem);
            await dbContext.SaveChangesAsync(stoppingToken);
            await cache.RemoveAsync($"task:{taskId}");

            _logger.LogInformation("Background worker processed task {TaskId}", taskId);
        }
    }
}
