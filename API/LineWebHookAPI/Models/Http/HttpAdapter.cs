using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LineWebHookAPI.Models.Http;

/// <summary>
/// http操作クラス
/// </summary>
public class HttpAdapter
{
    /// <summary>
    /// HttpClientオブジェクト
    /// </summary>
    protected HttpClient HttpClient { get;}

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
    public async virtual Task<T> GetAsync<T>(string url, AuthenticationHeaderValue authenticationHeaderValue = null) where T : class
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
    public async virtual Task<T> PostAsync<T, Tbody>(string url, Tbody body, AuthenticationHeaderValue authenticationHeaderValue = null)  where T : class
    {
        var content = JsonSerializer.Serialize(body);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = authenticationHeaderValue;
        Console.WriteLine(content);
        Console.WriteLine(authenticationHeaderValue);
        var res = await HttpClient.SendAsync(request);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException(res.RequestMessage.ToString());
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
    }
}
