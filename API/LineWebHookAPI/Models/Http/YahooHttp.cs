using LineWebHookAPI.Constants.Yahoo;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;
using LineWebHookAPI.Models.Dto.Yahoo;

namespace LineWebHookAPI.Models.Http;

/// <summary>
/// YahooAPI用のHTTPクライアント
/// </summary>
public class YahooHttp(HttpAdapter http,IConfiguration configuration) : ApiClient(http, configuration)
{
    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="dto">位置情報、ジャンルコード</param>
    /// <returns>実行結果</returns>
    public async Task<LocalDto> GetLocateAsync(Message message, string genreCode)
    {
        var genreQuery = genreCode is null ? "" : $"&gc={genreCode}";
        var url = string.Format
        (
            $"{Configuration.GetValue<string>("Yahoo:Url")}{message.CreateYahooApiQuey()}{genreQuery}&dist=1&results=20",
            YahooUrlRoot.LocalSearch,
            Configuration.GetValue<string>("Yahoo:Key")
        );
        return await Http.GetAsync<LocalDto>(url);
    }
}
