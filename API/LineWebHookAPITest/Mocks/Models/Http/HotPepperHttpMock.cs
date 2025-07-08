using LineDevSdk.DTOs.Commons.Messages;
using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.HotPeppers;
using LineWebHookAPI.Models.Http;

namespace LineWebHookAPITest.Mocks.Models.Http;

public class HotPepperHttpMock(HotPepperGourmetResponseDto getGourmetAsyncMock = null) : IHotPepperHttp
{
    private HotPepperGourmetResponseDto GetGourmetAsyncMock { get; } = getGourmetAsyncMock;

    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="dto">位置情報、ジャンルコード</param>
    /// <returns>実行結果</returns>
    public async Task<HotPepperGourmetResponseDto> GetGourmetAsync(IMessage message, GenreCode genreCode)
    {
        return await Task.Run(() => GetGourmetAsyncMock);
    }
}
