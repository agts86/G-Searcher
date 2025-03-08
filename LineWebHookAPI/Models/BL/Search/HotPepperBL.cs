using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.HotPepper;
using System.Text.Json;
using LineWebHookAPI.Models.DB;

namespace LineWebHookAPI.Models.BL.HotPepper;

public class HotPepperBL(IConfiguration configuration, MyContext dbContext)
{
    public HotPepperRepository HotPepperRepository { get; protected set; } = new HotPepperRepository(dbContext);

    public HttpClient HttpClient { get; protected set; } = new HttpClient();

    public string BaseUrl { get; protected set; } = $"{configuration.GetValue<string>("Api:Url")}?key={configuration.GetValue<string>("Api:Key")}&format=json";

    public JsonSerializerOptions JsonSerializerOptions { get; protected set; } = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

    public async Task<HotPepperGourmetResponseDto> GetGourmetAsync(GourmetGettingDto gourmetGettingDto)
    {
        await HotPepperRepository.CreateLogAsync(gourmetGettingDto);
        await HotPepperRepository.SaveChangesAsync();
        var url = $"{BaseUrl}&lat={gourmetGettingDto.Lat}&lng={gourmetGettingDto.Lng}";
        var result = await HttpClient.GetAsync(url);
        if (!result.IsSuccessStatusCode) throw new HttpRequestException(result.RequestMessage.ToString());
        var json = await result.Content.ReadAsStringAsync();
        var ret = JsonSerializer.Deserialize<HotPepperGourmetResponseDto>(json, JsonSerializerOptions);
        ret.Results.CheckResult();
        return ret;
    }
}
