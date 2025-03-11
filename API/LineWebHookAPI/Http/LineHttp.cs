using System.Net.Http.Headers;
using LineWebHookAPI.Models.Dto.Line.API.Requests;

namespace LineWebHookAPI.Http;

/// <summary>
/// ホットペッパーAPI用のHTTPクライアント
/// </summary>
public class LineHttp(HttpAdapter http,IConfiguration configuration) : ApiClient(http, configuration)
{
    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <returns>実行結果</returns>
    public async Task PostReplayAsync(Replay replay)
    {
        var url = string.Format(Configuration.GetValue<string>("Line:Url"),nameof(replay));
        var auth = new AuthenticationHeaderValue("Bearer", Configuration.GetValue<string>("Line:Token"));
        await Http.PostAsync<object,Replay>(url, replay, auth);
    }
}
