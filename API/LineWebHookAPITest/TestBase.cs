using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Models.DB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;

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
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options
    );

    /// <summary>
    /// 環境情報
    /// </summary>
    protected IWebHostEnvironment Env { get; } = new WebHostEnvironment();

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
