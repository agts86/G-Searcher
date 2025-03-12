using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LineWebHookAPI.Http;

/// <summary>
/// http操作クラス
/// </summary>
public class HttpAdapter
{
    /// <summary>
    /// HttpClientオブジェクト
    /// </summary>
    private HttpClient HttpClient { get;}

    /// <summary>
    /// シリアライズオプション
    /// </summary>
    private JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public HttpAdapter(int timeout = 100000)
    {
        HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMilliseconds(timeout)
        };
    }

    /// <summary>
    /// コンストラクタ
    /// CookieとかAllowRedirectとかはここで設定する
    /// </summary>
    public HttpAdapter(HttpMessageHandler httpMessageHandler, int timeout = 100000)
    {
        HttpClient = new HttpClient(httpMessageHandler)
        {
            Timeout = TimeSpan.FromMilliseconds(timeout)
        };
    }

    /// <summary>
    /// Getリクエストをする
    /// </summary>
    /// <param name="url">リクエスト先</param>
    /// <param name="authenticationHeaderValue">basic認証</param>
    /// <returns>レスポンス結果</returns>
    public async Task<T> GetAsync<T>(string url, AuthenticationHeaderValue authenticationHeaderValue = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
        };
        request.Headers.Authorization = authenticationHeaderValue;
        var res = await HttpClient.SendAsync(request);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException(res.RequestMessage.ToString());
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
    }

    /// <summary>
    /// Postリクエストをする
    /// </summary>
    /// <param name="url">リクエスト先</param>
    /// <param name="contentType">ContentType</param>
    /// <param name="authenticationHeaderValue">basic認証</param>
    /// <returns>レスポンス結果</returns>
    public async Task<T> PostAsync<T, Tbody>(string url, Tbody body, AuthenticationHeaderValue authenticationHeaderValue = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = authenticationHeaderValue;
        var res = await HttpClient.SendAsync(request);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException(res.RequestMessage.ToString());
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
    }
}
