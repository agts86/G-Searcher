using G_Searcher.Models;
using Microsoft.AspNetCore.Mvc;

namespace G_Searcher.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController(IConfiguration configuration) : ControllerBase
{
    public RestaurantApi RestaurantApi { get; protected set; } = new RestaurantApi(configuration);

    [HttpGet]
    public async Task<IActionResult> GetRestaurantByCurrentLocation([FromQuery] double lat,double lng)
    {
        var res = await RestaurantApi.GetRestaurantByCurrentLocation(lat,lng);
        return Ok(new {res.Results});
    }
}
