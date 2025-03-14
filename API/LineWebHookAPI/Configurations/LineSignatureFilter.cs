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
        // if (Environment.IsDevelopment())
        // {
        //     await next();
        //     return;
        // }

        var request = context.HttpContext.Request;
        if (!request.Headers.TryGetValue("x-line-signature", out var signatureHeader))
        {
            Console.WriteLine("Signature header is missing.");
            context.Result = new UnauthorizedResult();
            return;
        }
        Console.WriteLine($"Received Signature: {signatureHeader}");
        var channelSecret = Configuration.GetValue<string>("Line:ChannelSecret");
        Console.WriteLine($"Channel Secret: {channelSecret}");
        request.Body.Position = 0;
        using var StreamReader = new StreamReader(request.Body);
        var requestBody = await StreamReader.ReadToEndAsync();
        // requestBody = requestBody.Replace("\r\n", "\n");
        request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody)); // 再読込可能に
        request.Body.Position = 0;
        if (!VerifySignature(signatureHeader, requestBody, channelSecret))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        await next();
    }

    /// <summary>
    /// 検証メカニズム
    /// </summary>
    /// <param name="channelSecret"></param>
    /// <param name="requestBody"></param>
    /// <param name="receivedSignature"></param>
    /// <returns></returns>
    private static bool VerifySignature(string xLineSignature, string requestBody, string channelSecret)
    {
        var key = Encoding.UTF8.GetBytes(channelSecret);
        var body = Encoding.UTF8.GetBytes(requestBody);
        
        //channel secretをキーにしてHMAC-SHA256アルゴリズムでrequest bodyのダイジェスト値を得る
        using (HMACSHA256 hmac = new HMACSHA256(key))
        {
            var hash = hmac.ComputeHash(body, 0, body.Length);
            //ダイジェスト値をBASE64に変換
            var hash64 = Convert.ToBase64String(hash);
            //X-LINE-Signatureヘッダの値と一致すればOK！！
            return xLineSignature == hash64;
        }
    }
}
