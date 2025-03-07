using System.Net;

namespace G_Searcher.Models.Exceptions;

/// <summary>
/// ステータスコード例外の基底クラス
/// 基本的400台を想定
/// </summary>
public abstract class StatusCodeException(ResponseError error) : Exception
{
    /// <summary>
    /// ステータスコード
    /// </summary>
    public abstract HttpStatusCode StatusCode { get; }

    /// <summary>
    /// エラーメッセージクラス
    /// </summary>
    public ResponseError Error { get; } = error;
}
