using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.HotPeppers;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;

namespace LineWebHookAPI.Models.Http;

/// <summary>
/// ホットペッパーAPI用のHTTPクライアント
/// </summary>
public class HotPepperHttp(HttpAdapter http,IConfiguration configuration) : ApiClient(http, configuration)
{
    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <returns>実行結果</returns>
    public async Task<HotPepperGourmetResponseDto> GetGourmetAsync(LocationMessage message)
    {
        var midnightQuery = DateTime.Now.Hour >= 23 ? "&midnight=1" : "";
        var url = string.Format
        (
            $"{Configuration.GetValue<string>("HotPepper:Url")}&genre=G013&lat={message.Latitude}&lng={message.Longitude}{midnightQuery}",
            HotPepperUrlRoot.Gourmet,
            Configuration.GetValue<string>("HotPepper:Key")
        );
        var ret = await Http.GetAsync<HotPepperGourmetResponseDto>(url);
        ret.Results.CheckResult();
        return ret;
    }
}
