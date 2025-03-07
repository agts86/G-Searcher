using G_Searcher.Models.Dto;
using System.Text.Json;

namespace G_Searcher.Models;

public class RestaurantApi(IConfiguration configuration)
{
    public HttpClient HttpClient { get; protected set; } = new HttpClient();

    public string BaseUrl { get; protected set; } = $"{configuration.GetValue<string>("Api:Url")}?key={configuration.GetValue<string>("Api:Key")}&format=json";

    public async Task<RestaurantDto> GetRestaurantByCurrentLocation(double lat,double lng)
    {
        var url = $"{BaseUrl}&lat={lat}&lng={lng}";
        var result = await HttpClient.GetAsync(url);
        if(!result.IsSuccessStatusCode) throw new HttpRequestException(result.RequestMessage.ToString());
        var json = await result.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RestaurantDto>(json);
    }
}
