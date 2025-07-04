using LineWebHookAPI.Models.Dto.Line.Hook.Messages;
using LineWebHookAPI.Models.Dto.Yahoo;
using LineWebHookAPI.Models.Http;

namespace LineWebHookAPITest.Mocks.Models.Http;

public class YahooHttpMock(LocalDto getLocateAsyncMock = null) : IYahooHttp
{
    private LocalDto GetLocateAsyncMock { get; } = getLocateAsyncMock;

    public async Task<LocalDto> GetLocateAsync(Message message, string genreCode)
    {
        return await Task.Run(() => GetLocateAsyncMock);
    }
}
