using System.Net;

namespace G_Searcher.Models.Exceptions;

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
