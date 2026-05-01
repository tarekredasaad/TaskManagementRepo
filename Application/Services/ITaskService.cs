using Application.Dtos;

namespace Application.Services;

public interface ITaskService
{
    Task<TaskDto> CreateTaskAsync(Guid userId, CreateTaskRequest request);
    Task<TaskDto?> GetTaskByIdAsync(Guid userId, Guid taskId);
    Task<List<TaskDto>> GetMyTasksAsync(Guid userId);
    Task<TaskDto?> UpdateTaskStatusAsync(Guid userId, Guid taskId, UpdateTaskStatusRequest request);
}
