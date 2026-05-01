using Domain.Enum;

namespace Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Domain.Enum.TaskStatus Status { get; set; } = Domain.Enum.TaskStatus.Pending;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
