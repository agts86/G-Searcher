using System.Net;

namespace G_Searcher.Models.Exceptions;

public class BadRequestException(ResponseError error) : StatusCodeException(error)
{
    public override HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;
}
