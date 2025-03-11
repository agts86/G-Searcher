using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.HotPeppers;
using System.Text.Json;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Http;
using LineWebHookAPI.Models.Dto.Line.API.Requests;

namespace LineWebHookAPI.Models.BL.HotPeppers;

public class HotPepperBL(IConfiguration configuration, MyContext dbContext)
{
    public HotPepperRepository HotPepperRepository { get; protected set; } = new HotPepperRepository(dbContext);

    private IConfiguration Configuration { get; } = configuration;

    public HttpAdapter Http { get; protected set; } = new HttpAdapter();

    public async Task<Replay> PostGourmetLocationAsync(GourmetGettingDto gourmetGettingDto)
    {
        var message = gourmetGettingDto.Events.First().Message;
        
        await HotPepperRepository.CreateLogAsync(message);
        await HotPepperRepository.SaveChangesAsync();
        
        var hotPepperHttp = new HotPepperHttp(Http, Configuration);
        var gourmet = await hotPepperHttp.GetGourmetAsync(message);
        var lineHttp = new LineHttp(Http, Configuration);
        var replay = new Replay()
        {
            ReplyToken = gourmetGettingDto.Events.First().ReplyToken,
            Messages = new TemplateMessage[]
            {
                new()
                {
                    AltText = "検索結果",
                    Template = new CarouselTemplate()
                    {
                        Columns = gourmet.Results.ToCarouselTemplateColumns()
                    }
                }
            }
        };
        await lineHttp.PostReplayAsync(replay);
        return replay;
    }
}
