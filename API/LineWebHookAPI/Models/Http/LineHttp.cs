using System.Net.Http.Headers;
using LineWebHookAPI.Models.Dto.Line.API.Requests;

namespace LineWebHookAPI.Models.Http;

public interface ILineHttp
{
    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="reply">返信内容</param>
    /// <returns>実行結果</returns>
    Task PostReplyAsync(Reply reply);
}   

/// <summary>
/// LineAPI用のHTTPクライアント
/// </summary>
public class LineHttp(HttpClient httpClient, IConfiguration configuration) : ILineHttp
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
    /// <param name="message">位置情報</param>
    /// <returns>実行結果</returns>
    public async Task PostReplyAsync(Reply Reply)
    {
        var url = string.Format(Configuration.GetValue<string>("Line:Url"), "reply");
        var auth = new AuthenticationHeaderValue("Bearer", Configuration.GetValue<string>("Line:Token"));
        await Http.PostAsync<object, Reply>(url, Reply, auth);
    }
}
