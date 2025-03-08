namespace LineWebHookAPI.Models.Dto.HotPepper;

/// <summary>
/// GetGourmetAsyncのリクエストDto
/// </summary>
public class GourmetGettingDto
{
    /// <summary>
    /// 緯度
    /// </summary>
    public double Lat {get; set;}

    /// <summary>
    /// 軽度
    /// </summary>
    public double Lng {get; set;}
}
