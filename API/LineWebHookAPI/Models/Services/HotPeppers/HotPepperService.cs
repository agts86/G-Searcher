using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Models.Dto.Line.API.Requests;
using LineWebHookAPI.Constants.Line.API;
using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;
using LineWebHookAPI.Models.Dto.Line.Hook;

namespace LineWebHookAPI.Models.Services.HotPeppers;

/// <summary>
/// ホットペッパーコントローラーのビジネスロジック
/// </summary>
public interface IHotPepperService
{
    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    Task<Reply[]> PostGourmetLocationAsync(GourmetGettingDto gourmetGettingDto, GenreCode genreCode);
}

/// <summary>
/// ホットペッパーコントローラーのビジネスロジック
/// </summary>
public class HotPepperService(IConfiguration configuration, HotPepperRepositoryBase hotPepperRepository,IHostEnvironment env,IHttpAdapter http) : IHotPepperService
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    protected HotPepperRepositoryBase HotPepperRepository { get; set; } = hotPepperRepository;

    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// 環境情報
    /// </summary>
    private IHostEnvironment Env { get; } = env;

    /// <summary>
    /// Http操作クラス
    /// </summary>
    protected IHttpAdapter Http { get; set; } = http;

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    public async Task<Reply[]> PostGourmetLocationAsync(GourmetGettingDto gourmetGettingDto, GenreCode genreCode)
    {
        var hotPepperHttp = new HotPepperHttp(Http, Configuration);
        var lineHttp = new LineHttp(Http, Configuration);
        var replies = new List<Reply>();

        foreach(var e in gourmetGettingDto.Events ?? [])
        {
            if(e.Message is LocationMessage message) 
            {
                await HotPepperRepository.CreateGourmetLogAsync(message);
                await HotPepperRepository.SaveChangesAsync();
            }
            
            var gourmet = await hotPepperHttp.GetGourmetAsync(e.Message,genreCode);
            var columns = gourmet.Results.ToCarouselTemplateColumns();
            
            replies.Add
            (
                new Reply()
                {
                    ReplyToken = e.ReplyToken,
                    Messages = 
                    [
                        columns.Length > 0 ? 
                        new TemplateMessage()
                        {
                            AltText = "検索結果",
                            Template = new CarouselTemplate()
                            {
                                Columns = columns
                            }
                        }:
                        new TextV2Message()
                        {
                            Text = MessageTexts.NotFound
                        }
                    ] 
                }
            );
        }   
        
        // デバッグ実行時はエラーコード確定のため処理しない
        if(!Env.IsDevelopment()) replies.ForEach(async x => await lineHttp.PostReplyAsync(x));
        return [.. replies];
    }
}
