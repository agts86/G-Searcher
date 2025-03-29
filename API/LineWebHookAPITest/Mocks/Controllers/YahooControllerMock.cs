using LineWebHookAPI.Controllers;
using LineWebHookAPI.Models.BL.Yahoo;
using LineWebHookAPI.Models.DB;
using LineWebHookAPITest.Mocks.Models.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LineWebHookAPITest.Mocks.Controllers;

public class YahooControllerMock : YahooController
{
    public YahooControllerMock
    (
        IConfiguration configuration,
        LineWebHookContext dbContext,
        IHostEnvironment env,
        HttpAdapterMock http
    ) : base(configuration, dbContext, env)
    {
        YahooBL = new YahooBL(configuration, dbContext, env, http);
    }
}
