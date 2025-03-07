using G_Searcher.DB;
using G_Searcher.Models.BL.Search;
using G_Searcher.Models.Dto.Search;
using Microsoft.AspNetCore.Mvc;

namespace G_Searcher.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController
(
    IConfiguration configuration,
    MyContext dbContext
) : ControllerBase
{
    public SearchBL SearchBL { get; protected set; } = new SearchBL(configuration,dbContext);

    [HttpGet]
    public async Task<IActionResult> GetGourmetAsync([FromQuery] GourmetGettingDto gourmetGettingDto)
    {
        var res = await SearchBL.GetGourmetAsync(gourmetGettingDto);
        return Ok(new {res.Results});
    }
}
