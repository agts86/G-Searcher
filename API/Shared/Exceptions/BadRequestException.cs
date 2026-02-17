using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Shared.Exceptions;

/// <summary>
/// BadRequest例外
/// </summary>
[ExcludeFromCodeCoverage]
public class BadRequestException(ResponseError error) : StatusCodeException(error)
{
    /// <summary>
    /// ステータスコード
    /// </summary>

    public override HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;
}
