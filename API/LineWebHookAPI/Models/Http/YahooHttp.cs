using LineWebHookAPI.Constants.Yahoo;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;
using LineWebHookAPI.Models.Dto.Yahoo;

namespace LineWebHookAPI.Models.Http;

public interface IYahooHttp
{
    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>実行結果</returns>
    Task<LocalDto> GetLocateAsync(Message message, string genreCode);
}

/// <summary>
/// YahooAPI用のHTTPクライアント
/// </summary>
public class YahooHttp(HttpClient httpClient, IConfiguration configuration) : IYahooHttp
{

    /// <summary>
    /// HttpClient
    /// </summary>
    private HttpAdapter Http { get; } = new HttpAdapter(httpClient);

    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

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
