namespace LineWebHookAPI.Models.Dto.Line.Messages;

/// <summary>
/// 位置情報メッセージ
/// </summary>
public class LocationMessage : Message
{
    /// <summary>
    /// タイトル
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 住所
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// 緯度
    /// </summary>
    public double Latitude {get; set;}

    /// <summary>
    /// 軽度
    /// </summary>
    public double Longitude {get; set;}
}
