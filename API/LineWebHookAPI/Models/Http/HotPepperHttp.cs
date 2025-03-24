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
    /// <param name="dto">位置情報、ジャンルコード</param>
    /// <returns>実行結果</returns>
    public async Task<HotPepperGourmetResponseDto> GetGourmetAsync(Message message,GenreCode genreCode)
    {
        var hour = DateTime.Now.Hour;
        var midnightQuery = hour <= 5 && 23 <= hour ? "&midnight=1" : "";
        var genreQuery = Enum.IsDefined(genreCode) ? $"&genre={genreCode}" : "";
        var url = string.Format
        (
            $"{Configuration.GetValue<string>("HotPepper:Url")}{message.CreateHotPepperApiQuey()}{genreQuery}{midnightQuery}",
            HotPepperUrlRoot.Gourmet,
            Configuration.GetValue<string>("HotPepper:Key")
        );
        var ret = await Http.GetAsync<HotPepperGourmetResponseDto>(url);
        ret.Results.CheckResult();
        return ret;
    }
}
