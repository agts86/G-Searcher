using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace LineWebHookAPI.Models.Job;

/// <summary>
/// バックグラウンドジョブキューインターフェース
/// </summary>
/// <typeparam name="T">ジョブの型</typeparam>
public interface IBackgroundJobQueue<T>
{
    /// <summary>
    /// ジョブをキューに追加する
    /// </summary>
    /// <param name="job">ジョブ</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>ジョブをキューに追加するタスク</returns>
    ValueTask EnqueueAsync(T job, CancellationToken ct = default);

    /// <summary>
    /// キューからすべてのジョブを非同期に読み取る
    /// </summary>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>ジョブの列挙子</returns>
    IAsyncEnumerable<T> ReadAllAsync(CancellationToken ct = default);
}

/// <summary>
/// バックグラウンドジョブキュー実装
/// </summary>
/// <typeparam name="T">ジョブの型</typeparam>
public class BackgroundJobQueue<T> : IBackgroundJobQueue<T>
{
    /// <summary>
    /// チャネル
    /// </summary>
    /// <typeparam name="T">ジョブの型</typeparam>
    private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();

    /// <summary>
    /// ジョブをキューに追加する
    /// </summary>
    /// <param name="job">ジョブの型</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>ジョブをキューに追加するタスク</returns>
    public ValueTask EnqueueAsync(T job, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(job, ct);

    /// <summary>
    /// キューからすべてのジョブを非同期に読み取る
    /// </summary>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>ジョブの列挙子</returns>
    public IAsyncEnumerable<T> ReadAllAsync(CancellationToken ct = default)
    {
        return GetAllJobsAsync(ct);
    }

    /// <summary>
    /// ジョブを非同期に取得する
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async IAsyncEnumerable<T> GetAllJobsAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        while (await _channel.Reader.WaitToReadAsync(ct))
        {
            while (_channel.Reader.TryRead(out var job))
                yield return job;
        }
    }
}