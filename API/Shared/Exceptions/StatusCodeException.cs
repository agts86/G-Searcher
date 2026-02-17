using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Shared.Exceptions;

/// <summary>
/// ステータスコード例外の基底クラス
/// 基本的400台を想定
/// </summary>
[ExcludeFromCodeCoverage]
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
