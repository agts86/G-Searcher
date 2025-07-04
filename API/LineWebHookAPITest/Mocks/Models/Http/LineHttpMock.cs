using LineWebHookAPI.Models.Dto.Line.API.Requests;
using LineWebHookAPI.Models.Http;

namespace LineWebHookAPITest.Mocks.Models.Http;

public class LineHttpMock() : ILineHttp
{

    public async Task PostReplyAsync(Reply Reply)
    {
        await Task.Run(() => { });
    }
}
