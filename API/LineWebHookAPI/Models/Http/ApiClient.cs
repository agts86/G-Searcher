namespace LineWebHookAPI.Models.Http;

/// <summary>
/// API通信クライアント基底クラス
/// HttpClientは使い回すほうが効率がいいため、差し込み型にする
/// </summary>
public class ApiClient(HttpAdapter http,IConfiguration configuration)
{
    /// <summary>
    /// HttpClient
    /// </summary>
    protected HttpAdapter Http { get; } = http;

    /// <summary>
    /// 設定情報
    /// </summary>
    protected IConfiguration Configuration { get; } = configuration;
}
