using LineDevSdk.DTOs.Commons.Messages;
using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.HotPeppers;

namespace LineWebHookAPI.Models.Http;


public interface IHotPepperHttp
{
    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>実行結果</returns>
    Task<HotPepperGourmetResponseDto> GetGourmetAsync(IMessage message, GenreCode genreCode);
}

/// <summary>
/// ホットペッパーAPI用のHTTPクライアント
/// </summary>
public class HotPepperHttp(HttpClient httpClient, IConfiguration configuration) : IHotPepperHttp
{
    /// <summary>
    /// HttpClient
    /// </summary>
    protected HttpAdapter Http { get; } = new HttpAdapter(httpClient);

    /// <summary>
    /// 設定情報
    /// </summary>
    protected IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="dto">位置情報、ジャンルコード</param>
    /// <returns>実行結果</returns>
    public async Task<HotPepperGourmetResponseDto> GetGourmetAsync(IMessage message, GenreCode genreCode)
    {
        const int japanKind = 9;
        var hour = DateTime.UtcNow.AddHours(japanKind).Hour;
        var midnightQuery = hour <= 5 || 23 <= hour ? "&midnight=1" : "";
        var genreQuery = Enum.IsDefined(genreCode) ? $"&genre={genreCode}" : "";
        var url = string.Format
        (
            $"{Configuration.GetValue<string>("HotPepper:Url")}{CreateHotPepperApiQuey(message)}{genreQuery}{midnightQuery}",
            HotPepperUrlRoot.Gourmet,
            Configuration.GetValue<string>("HotPepper:Key")
        );
        var ret = await Http.GetAsync<HotPepperGourmetResponseDto>(url);
        ret.Results.CheckResult();
        return ret;
    }

    private static string CreateHotPepperApiQuey(IMessage message)
    {
        var ret = string.Empty;
        if (message is LocationMessage locationMessage)
            ret = $"&lat={locationMessage.Latitude}&lng={locationMessage.Longitude}";
        else if (message is TextMessage textMessage)
        {
            var value = textMessage.Text.Replace('　', ' ');
            ret =  $"&keyword={Uri.EscapeDataString(value)}";
        }
        return ret;
    }
}
