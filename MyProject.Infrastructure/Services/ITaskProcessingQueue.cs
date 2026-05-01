namespace MyProject.Infrastructure.Services;

public interface ITaskProcessingQueue
{
    ValueTask EnqueueAsync(Guid taskId, CancellationToken cancellationToken = default);
    ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken);
}
