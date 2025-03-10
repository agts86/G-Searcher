using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.BL.HotPeppers;
using LineWebHookAPI.Models.Dto.HotPeppers;
using Microsoft.AspNetCore.Mvc;

namespace LineWebHookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotPepperController
(
    IConfiguration configuration,
    MyContext dbContext
) : ControllerBase
{
    public HotPepperBL HotPepperBL { get; protected set; } = new HotPepperBL(configuration,dbContext);

    [HttpPost("Gourmet/location")]
    public async Task<IActionResult> PostGourmetLocationAsync([FromBody] GourmetGettingDto gourmetGettingDto)
    {
        var res = await HotPepperBL.PostGourmetLocationAsync(gourmetGettingDto);
        return Ok(res);
    }
}
