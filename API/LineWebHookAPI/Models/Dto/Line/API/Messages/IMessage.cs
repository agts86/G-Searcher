namespace LineWebHookAPI.Models.Dto.Line.API.Messages;

/// <summary>
/// メッセージインターフェイス
/// </summary>
public interface IMessage
{
    /// <summary>
    /// タイプ
    /// </summary>
    string Type { get; }
}
