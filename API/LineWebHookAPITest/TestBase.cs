using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Models.DB;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Moq;
using LineWebHookAPI.Models.Http;
using LineDevSdk.DTOs.Commons.Messages;
using LineWebHookAPI.Models.Dto.Yahoo;
using LineDevSdk.Https;
using LineDevSdk.DTOs.MessagingAPIs;
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
    /// YahooHttpのモックを作成します。
    /// </summary>
    /// <param name="ret">返却値</param>
    /// <returns>YahooHttpのモック</returns>
    protected static IYahooHttp CreateYahooHttpMock(LocalDto ret)
    {
        var yahooHttpMock = new Mock<IYahooHttp>();
        yahooHttpMock.Setup(x => x.GetLocateAsync(It.IsAny<IMessage>(), It.IsAny<string>()))
            .ReturnsAsync(ret);
        return yahooHttpMock.Object;
    }

    /// <summary>
    /// LineHttpのモックを作成します。
    /// </summary>
    /// <returns>LineHttpのモック</returns>
    protected static ILineHttp CreateLineHttpMock()
    {
        var lineHttpMock = new Mock<ILineHttp>();
        lineHttpMock.Setup(x => x.PostReplyAsync(It.IsAny<Reply>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        return lineHttpMock.Object;
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
