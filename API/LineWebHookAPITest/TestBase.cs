using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Models.DB;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Moq;
using LineDevSdk.Http;
using LineDevSdk.DTO.MessagingAPIs;
using YahooDeveloperApiClient.YOLP;
using YahooDeveloperApiClient.YOLP.Request;
using YahooDeveloperApiClient.YOLP.Response;
namespace LineWebHookAPITest;

public abstract class TestBase
{
    /// <summary>
    /// 日本のタイムゾーン
    /// </summary>
    protected const int JapanKind = 9;

    /// <summary>
    /// DB
    /// </summary>
    protected LineWebHookContext DbContext { get; set; } = new LineWebHookContext
    (
        new DbContextOptionsBuilder<LineWebHookContext>()
        .UseSqlite("Filename=:memory:")
        .Options
    );

    /// <summary>
    /// 環境情報
    /// </summary>
    protected IWebHostEnvironment Env { get; } = new WebHostEnvironment();

    /// <summary>
    /// 設定情報
    /// </summary>
    protected IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .AddInMemoryCollection
            (
                new Dictionary<string, string>
                {
                    { "Auth:AdminUserName", "admin" },
                    { "Auth:AdminPassword", "admin" },
                    { "Auth:JwtKey", "DevelopmentOnlyJwtKeyMustBeAtLeast32Chars" },
                    { "Auth:Issuer", "LineWebHookAPI" },
                    { "Auth:Audience", "LineWebHookAdmin" },
                    { "Auth:ExpiresMinutes", "120" }
                }
            )
            .Build();
    /// <summary>
    /// テスト用の初期化処理を行います。
    /// </summary>
    public TestBase()
    {
        DbContext.Database.OpenConnection();
        DbContext.Database.EnsureCreated();
    }

    /// <summary>
    /// YahooClientのモックを作成します。
    /// </summary>
    /// <param name="ret">返却値</param>
    /// <returns>YahooClientのモック</returns>
    protected static IYOLPClient CreateYahooClientMock(LocalSearchResult ret)
    {
        var YahooClientMock = new Mock<IYOLPClient>();
        YahooClientMock.Setup(x => x.GetLocalSearchResultAsync(It.IsAny<LocalSearchRequest>()))
            .ReturnsAsync(ret);
        return YahooClientMock.Object;
    }

    /// <summary>
    /// LineMessagingClientのモックを作成します。
    /// </summary>
    /// <returns>LineMessagingClientのモック</returns>
    protected static ILineMessagingClient CreateLineMessagingClientMock()
    {
        var LineMessagingClientMock = new Mock<ILineMessagingClient>();
        LineMessagingClientMock.Setup(x => x.PostReplyAsync(It.IsAny<Reply>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        return LineMessagingClientMock.Object;
    }

    /// <summary>
    /// IHostEnvironment実装クラス
    /// </summary>
    protected class WebHostEnvironment : IWebHostEnvironment
    {
        /// <summary>
        /// 実行状態
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// アプリケーション名
        /// </summary> 
        public string ApplicationName { get; set; }

        /// <summary>
        /// ルートパス
        /// </summary> 
        public string ContentRootPath { get; set; }

        /// <summary>
        /// ファイルプロバイダ
        /// </summary> 
        public IFileProvider ContentRootFileProvider { get; set; }

        /// <summary>
        /// WebRootPath
        /// </summary>
        public string WebRootPath { get; set; }

        /// <summary>
        /// WebRootFileProvider
        /// </summary>
        public IFileProvider WebRootFileProvider { get; set; }
    }
}
