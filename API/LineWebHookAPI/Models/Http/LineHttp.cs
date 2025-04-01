using System.Net.Http.Headers;
using LineWebHookAPI.Models.Dto.Line.API.Requests;

namespace LineWebHookAPI.Models.Http;

/// <summary>
/// ホットペッパーAPI用のHTTPクライアント
/// </summary>
public class LineHttp(IHttpAdapter http,IConfiguration configuration) : ApiClient(http, configuration)
{
    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <returns>実行結果</returns>
    public async Task PostReplyAsync(Reply Reply)
    {
        var url = string.Format(Configuration.GetValue<string>("Line:Url"),"reply");
        var auth = new AuthenticationHeaderValue("Bearer", Configuration.GetValue<string>("Line:Token"));
        await Http.PostAsync<object,Reply>(url, Reply, auth);
    }
}
