using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Application.Models.Exceptions;

/// <summary>
/// Unauthorized例外
/// </summary>
[ExcludeFromCodeCoverage]
public class UnauthorizedException(ResponseError error) : StatusCodeException(error)
{
    /// <summary>
    /// ステータスコード
    /// </summary>
    public override HttpStatusCode StatusCode { get; } = HttpStatusCode.Unauthorized;
}
