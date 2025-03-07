using G_Searcher.Models.DB.Repositories;
using G_Searcher.Models.Dto.Search;
using System.Text.Json;
using G_Searcher.Models.DB;

namespace G_Searcher.Models.BL.Search;

public class SearchBL(IConfiguration configuration, MyContext dbContext)
{
    public SearchRepository SearchRepository { get; protected set; } = new SearchRepository(dbContext);

    public HttpClient HttpClient { get; protected set; } = new HttpClient();

    public string BaseUrl { get; protected set; } = $"{configuration.GetValue<string>("Api:Url")}?key={configuration.GetValue<string>("Api:Key")}&format=json";

    public JsonSerializerOptions JsonSerializerOptions { get; protected set; } = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

    public async Task<HotPepperGourmetResponseDto> GetGourmetAsync(GourmetGettingDto gourmetGettingDto)
    {
        await SearchRepository.CreateLogAsync(gourmetGettingDto);
        await SearchRepository.SaveChangesAsync();
        var url = $"{BaseUrl}&lat={gourmetGettingDto.Lat}&lng={gourmetGettingDto.Lng}";
        var result = await HttpClient.GetAsync(url);
        if (!result.IsSuccessStatusCode) throw new HttpRequestException(result.RequestMessage.ToString());
        var json = await result.Content.ReadAsStringAsync();
        var ret = JsonSerializer.Deserialize<HotPepperGourmetResponseDto>(json, JsonSerializerOptions);
        ret.Results.CheckResult();
        return ret;
    }
}
