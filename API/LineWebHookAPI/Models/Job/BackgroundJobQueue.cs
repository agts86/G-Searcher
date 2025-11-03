using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LineWebHookAPI.Models.Job;

public interface IBackgroundJobQueue<T>
{
    ValueTask EnqueueAsync(T job, CancellationToken ct = default);
    IAsyncEnumerable<T> ReadAllAsync(CancellationToken ct = default);
}

public class BackgroundJobQueue<T> : IBackgroundJobQueue<T>
{
    private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();

    public ValueTask EnqueueAsync(T job, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(job, ct);

    public IAsyncEnumerable<T> ReadAllAsync(CancellationToken ct = default)
    {
        return GetAllJobsAsync(ct);
    }

    private async IAsyncEnumerable<T> GetAllJobsAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        while (await _channel.Reader.WaitToReadAsync(ct))
        {
            while (_channel.Reader.TryRead(out var job))
                yield return job;
        }
    }
}