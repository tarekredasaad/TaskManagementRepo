using Domain.Entities;

namespace MyProject.Infrastructure.Repositories.Tasks;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<List<TaskItem>> GetUserTasks(Guid userId);
    Task<bool> ExistsByTitleForDayAsync(Guid userId, string title, DateTime dayUtc);
    void Update(TaskItem task);
}
