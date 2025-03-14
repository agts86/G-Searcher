using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace LineWebHookAPI.Configurations;

/// <summary>
/// LineAPIの署名検証フィルター
/// </summary>
public class LineSignatureFilter(IConfiguration configuration, IWebHostEnvironment environment): IAsyncActionFilter
{
    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// 環境変数
    /// </summary>
    private IWebHostEnvironment Environment { get; } = environment;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // デバッグ環境では署名検証をスキップ
        if (Environment.IsDevelopment())
        {
            await next();
            return;
        }

        var request = context.HttpContext.Request;
        if (!request.Headers.TryGetValue("x-line-signature", out var signatureHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var channelSecret = Configuration.GetValue<string>("Line:ChannelSecret");
        // 再度読み込むために一度0に戻す
        request.Body.Position = 0;
        using var StreamReader = new StreamReader(request.Body,Encoding.UTF8);
        var requestBody = await StreamReader.ReadToEndAsync();
        // なんかあったときのためにポジションを0に戻す
        request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody))
        {
            Position = 0
        }; 

        if (!VerifySignature(channelSecret, requestBody, signatureHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        await next();
    }

    /// <summary>
    /// 検証メカニズム
    /// </summary>
    /// <param name="channelSecret">チャンネルシークレット</param>
    /// <param name="requestBody">Body文字列</param>
    /// <param name="receivedSignature">x-line-signatureの値</param>
    /// <returns>結果</returns>
    private static bool VerifySignature(string channelSecret, string requestBody, string receivedSignature)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(channelSecret));
        var requestBodyBytes = Encoding.UTF8.GetBytes(requestBody);
        var hash = hmac.ComputeHash(requestBodyBytes);
        var computedSignature = Convert.ToBase64String(hash);
        return computedSignature == receivedSignature;
    }
}
