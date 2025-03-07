namespace G_Searcher.Models.Exceptions;

/// <summary>
/// エラーメッセージ用クラス
/// </summary>
public class ResponseError(string message)
{
    /// <summary>
    /// エラーメッセージ
    /// </summary
    public string Message { get;} = message;
}
