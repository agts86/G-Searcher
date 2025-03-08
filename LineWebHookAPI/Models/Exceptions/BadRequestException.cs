using System.Net;

namespace LineWebHookAPI.Models.Exceptions;

/// <summary>
/// BadRequest例外
/// </summary>
public class BadRequestException(ResponseError error) : StatusCodeException(error)
{
    /// <summary>
    /// ステータスコード
    /// </summary>

    public override HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;
}
