using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.HotPeppers;
using System.Text.Json;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Dto.Line.API.Templates;
using LineWebHookAPI.Models.Dto.Line.API;

namespace LineWebHookAPI.Models.BL.HotPeppers;

public class HotPepperBL(IConfiguration configuration, MyContext dbContext)
{
    public HotPepperRepository HotPepperRepository { get; protected set; } = new HotPepperRepository(dbContext);

    public HttpClient HttpClient { get; protected set; } = new HttpClient();

    public string BaseUrl { get; protected set; } = $"{configuration.GetValue<string>("Api:Url")}?key={configuration.GetValue<string>("Api:Key")}&format=json";

    public JsonSerializerOptions JsonSerializerOptions { get; protected set; } = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

    public async Task<TemplateMessage> PostGourmetLocationAsync(GourmetGettingDto gourmetGettingDto)
    {
        var message = gourmetGettingDto.Events.First().Message;
        
        await HotPepperRepository.CreateLogAsync(message);
        await HotPepperRepository.SaveChangesAsync();
        
        var url = $"{BaseUrl}&lat={message.Latitude}&lng={message.Longitude}";
        var result = await HttpClient.GetAsync(url);
        if (!result.IsSuccessStatusCode) throw new HttpRequestException(result.RequestMessage.ToString());
        var json = await result.Content.ReadAsStringAsync();
        var gourmet = JsonSerializer.Deserialize<HotPepperGourmetResponseDto>(json, JsonSerializerOptions);
        gourmet.Results.CheckResult();
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
