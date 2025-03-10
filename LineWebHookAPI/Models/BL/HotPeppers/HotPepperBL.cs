using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.HotPeppers;
using System.Text.Json;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Dto.Line.API.Templates;
using LineWebHookAPI.Models.Dto.Line.API;
using LineWebHookAPI.Http;

namespace LineWebHookAPI.Models.BL.HotPeppers;

public class HotPepperBL(IConfiguration configuration, MyContext dbContext)
{
    public HotPepperRepository HotPepperRepository { get; protected set; } = new HotPepperRepository(dbContext);

    private IConfiguration Configuration { get; } = configuration;

    public HttpAdapter Http { get; protected set; } = new HttpAdapter();

    public async Task<TemplateMessage> PostGourmetLocationAsync(GourmetGettingDto gourmetGettingDto)
    {
        var message = gourmetGettingDto.Events.First().Message;
        
        await HotPepperRepository.CreateLogAsync(message);
        await HotPepperRepository.SaveChangesAsync();
        
        var HotPepperHttp = new HotPepperHttp(Http, Configuration);
        var gourmet = await HotPepperHttp.GetGourmetAsync(message);
        var columns = gourmet.Results.ToCarouselTemplateColumns();
        var templateMessage = new TemplateMessage()
        {
            AltText = "検索結果",
            Template = new CarouselTemplate()
            {
                Columns = columns
            }
        };
        return templateMessage;
    }
}
