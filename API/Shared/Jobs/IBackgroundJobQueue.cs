namespace Shared.Jobs;

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
