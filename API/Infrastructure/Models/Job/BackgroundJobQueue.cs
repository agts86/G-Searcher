using System.Threading.Channels;
using Shared.Jobs;

namespace Infrastructure.Models.Job;

/// <summary>
/// バックグラウンドジョブキュー実装
/// </summary>
/// <typeparam name="T">ジョブの型</typeparam>
internal class BackgroundJobQueue<T> : IBackgroundJobQueue<T>
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
    /// キューに溜まっているジョブをまとめて読み取る（最低1件を待ってから、その時点で読み取れる分をまとめて返す）
    /// </summary>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>ジョブのバッチ</returns>
    public async Task<IReadOnlyList<T>> ReadBatchAsync(CancellationToken ct = default)
    {
        await _channel.Reader.WaitToReadAsync(ct);

        var batch = new List<T>();
        while (_channel.Reader.TryRead(out var job))
            batch.Add(job);
        return batch;
    }
}
