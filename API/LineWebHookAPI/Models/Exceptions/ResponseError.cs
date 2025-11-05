using System.Diagnostics.CodeAnalysis;

namespace LineWebHookAPI.Models.Exceptions;

/// <summary>
/// エラーメッセージ用クラス
/// </summary>
[ExcludeFromCodeCoverage]
public class ResponseError(string message)
{
    /// <summary>
    /// エラーメッセージ
    /// </summary
    public string Message { get; } = message;
}
