using System.Threading.Channels;

namespace MyProject.Infrastructure.Services;

public class TaskProcessingQueue : ITaskProcessingQueue
{
    private readonly Channel<Guid> _queue = Channel.CreateUnbounded<Guid>();

    public ValueTask EnqueueAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return _queue.Writer.WriteAsync(taskId, cancellationToken);
    }

    public ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAsync(cancellationToken);
    }
}
