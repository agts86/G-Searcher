using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.BL.HotPepper;
using LineWebHookAPI.Models.Dto.HotPepper;
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

    [HttpGet]
    public async Task<IActionResult> GetGourmetAsync([FromQuery] GourmetGettingDto gourmetGettingDto)
    {
        var res = await HotPepperBL.GetGourmetAsync(gourmetGettingDto);
        return Ok(new {res.Results});
    }
}
