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
    /// キューに溜まっているジョブをまとめて読み取る（最低1件を待ってから、その時点で読み取れる分をまとめて返す）
    /// </summary>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>ジョブのバッチ</returns>
    Task<IReadOnlyList<T>> ReadBatchAsync(CancellationToken ct = default);
}
