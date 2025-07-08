using LineDevSdk.Https;
using LineDevSdk.DTOs.MessagingAPIs;

namespace LineWebHookAPITest.Mocks.Models.Http;

public class LineHttpMock() : ILineHttp
{
    public async Task PostReplyAsync(Reply Reply, string endPointUrl, string token)
    {
        await Task.Run(() => { });
    }
}
