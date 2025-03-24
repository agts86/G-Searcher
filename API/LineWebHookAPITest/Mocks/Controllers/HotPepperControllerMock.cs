using LineWebHookAPI.Controllers;
using LineWebHookAPI.Models.BL.HotPeppers;
using LineWebHookAPI.Models.DB;
using LineWebHookAPITest.Mocks.Models.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LineWebHookAPITest.Mocks.Controllers;

public class HotPepperControllerMock : HotPepperController
{
    public HotPepperControllerMock
    (
        IConfiguration configuration,
        LineWebHookContext dbContext,
        IHostEnvironment env,
        HttpAdapterMock http
    ) : base(configuration, dbContext, env)
    {
        HotPepperBL = new HotPepperBL(configuration, dbContext, env, http);
    }
}
