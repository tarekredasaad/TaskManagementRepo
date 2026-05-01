using Application.Dtos;
using Application.Services;
using MyProject.Infrastructure.CashService;
using MyProject.Infrastructure.Repositories.Tasks;

namespace MyProject.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly AppDbContext _dbContext;
    private readonly RedisCacheService _cache;
    private readonly ITaskProcessingQueue _queue;

    public TaskService(ITaskRepository taskRepository, AppDbContext dbContext,
        RedisCacheService cache,
        ITaskProcessingQueue queue)
    {
        _taskRepository = taskRepository;
        _dbContext = dbContext;
        _cache = cache;
        _queue = queue;
    }

    public async Task<TaskDto> CreateTaskAsync(Guid userId, CreateTaskRequest request)
    {
        var exists = await _taskRepository.ExistsByTitleForDayAsync(userId, request.Title.Trim(), DateTime.UtcNow);
        if (exists)
        {
            throw new InvalidOperationException("Duplicate task title for the same day is not allowed.");
        }

        var taskItem = new Domain.Entities.TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority,
            Status = Domain.Enum.TaskStatus.Pending,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(taskItem);
        await _dbContext.SaveChangesAsync();
        await _queue.EnqueueAsync(taskItem.Id);

        return ToDto(taskItem);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(Guid userId, Guid taskId)
    {
        var cacheKey = $"task:{taskId}";
        var cached = await _cache.GetAsync<TaskDto>(cacheKey);
        if (cached is not null && cached.UserId == userId)
        {
            return cached;
        }

        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task is null || task.UserId != userId)
        {
            return null;
        }

        var dto = ToDto(task);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15));
        return dto;
    }

    public async Task<List<TaskDto>> GetMyTasksAsync(Guid userId)
    {
        var tasks = await _taskRepository.GetUserTasks(userId);
        return tasks.Select(ToDto).ToList();
    }

    public async Task<TaskDto?> UpdateTaskStatusAsync(Guid userId, Guid taskId, UpdateTaskStatusRequest request)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task is null || task.UserId != userId)
        {
            return null;
        }

        task.Status = request.Status;
        _taskRepository.Update(task);
        await _dbContext.SaveChangesAsync();
        await _cache.RemoveAsync($"task:{taskId}");

        return ToDto(task);
    }

    private static TaskDto ToDto(Domain.Entities.TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Priority = task.Priority,
        Status = task.Status,
        CreatedAt = task.CreatedAt,
        UserId = task.UserId
    };
}
