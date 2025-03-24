using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Models.DB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;

namespace LineWebHookAPITest;

public abstract class TestBase
{
    /// <summary>
    /// DB
    /// </summary>
    protected LineWebHookContext DbContext { get; set; } = new LineWebHookContext
    (
        new DbContextOptionsBuilder<LineWebHookContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options
    );

    /// <summary>
    /// 設定情報
    /// </summary>
    protected IConfiguration Configuration { get; } = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
        { "Line:Url", "https://api.line.me/v2/bot/message/{0}" }
    })
    .Build();

    /// <summary>
    /// 環境情報
    /// </summary>
    protected IHostEnvironment Env { get; } = new HostEnvironment();

    /// <summary>
    /// IHostEnvironment実装クラス
    /// </summary>
    protected class HostEnvironment : IHostEnvironment
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
    }
}
