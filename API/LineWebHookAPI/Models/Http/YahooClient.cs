using System.Diagnostics.CodeAnalysis;
using LineDevSdk.DTOs.Commons.Messages;
using LineWebHookAPI.Constants.Yahoo;
using LineWebHookAPI.Models.Dto.Yahoo;

namespace LineWebHookAPI.Models.Http;

public interface IYahooClient
{
    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>実行結果</returns>
    Task<LocalDto> GetLocateAsync(IMessage message, string genreCode);
}

/// <summary>
/// YahooAPI用のHTTPクライアント
/// </summary>
[ExcludeFromCodeCoverage]
public class YahooClient(HttpClient httpClient, IConfiguration configuration) : IYahooClient
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
    public async Task<LocalDto> GetLocateAsync(IMessage message, string genreCode)
    {
        var genreQuery = genreCode is null ? "" : $"&gc={genreCode}";
        var url = string.Format
        (
            $"{Configuration.GetValue<string>("Yahoo:Url")}{CreateYahooApiQuey(message)}{genreQuery}&dist=1&results=20",
            YahooUrlRoot.LocalSearch,
            Configuration.GetValue<string>("Yahoo:Key")
        );
        return await Http.GetAsync<LocalDto>(url);
    }

    private static string CreateYahooApiQuey(IMessage message)
    {
        var ret = string.Empty;
        if (message is LocationMessage locationMessage)
            ret = $"&lat={locationMessage.Latitude}&lon={locationMessage.Longitude}";
        else if (message is TextMessage textMessage)
        {
            var value = textMessage.Text.Replace('　', ' ');
            ret = $"&query={Uri.EscapeDataString(value)}";
        }
        return ret;
    }
}
