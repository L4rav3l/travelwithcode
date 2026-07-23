using System.Threading.Channels;

namespace TravelWithCode.Infrastructure;

public record LxcTask(int containerId, string Token, string github);

public interface ILxcTaskQueue
{
    void QueueLxcSetup(int containerId, string Token, string github);
    ValueTask<LxcTask> DequeueAsync(CancellationToken cancellationToken);
}

public class LxcTaskQueue : ILxcTaskQueue
{
    private readonly Channel<LxcTask> _queue = Channel.CreateUnbounded<LxcTask>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });

    public void QueueLxcSetup(int containerId, string token, string github)
    {
        if(string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token));
        }

        var task = new LxcTask(containerId, token, github);
        _queue.Writer.TryWrite(task);
    }

    public async ValueTask<LxcTask> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }         
}