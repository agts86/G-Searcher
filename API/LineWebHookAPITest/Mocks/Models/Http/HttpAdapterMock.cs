using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LineWebHookAPI.Models.Dto.HotPeppers;
using LineWebHookAPI.Models.Http;

namespace LineWebHookAPITest.Mocks.Models.Http;

public class HttpAdapterMock(object getAsyncValue = null, object postAsyncValue = null) : HttpAdapter
{
    private object GetAsyncValue { get; } = getAsyncValue;
    private object PostAsyncValue { get; } = postAsyncValue;

    /// <summary>
    /// Getリクエストをする
    /// </summary>
    /// <param name="url">リクエスト先</param>
    /// <param name="authenticationHeaderValue">basic認証</param>
    /// <returns>レスポンス結果</returns>
    public async override Task<T> GetAsync<T>(string url, AuthenticationHeaderValue authenticationHeaderValue = null)
    {
        var ret = GetAsyncValue as T;
        return await Task.Run(() => ret);
    }

    /// <summary>
    /// Postリクエストをする
    /// </summary>
    /// <param name="url">リクエスト先</param>
    /// <param name="contentType">ContentType</param>
    /// <param name="authenticationHeaderValue">basic認証</param>
    /// <returns>レスポンス結果</returns>
    public async override Task<T> PostAsync<T, Tbody>(string url, Tbody body, AuthenticationHeaderValue authenticationHeaderValue = null)
    {
        var ret = PostAsyncValue as T;
        return await Task.Run(() => ret);
    }
}
