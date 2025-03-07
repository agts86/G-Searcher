using System.Net;

namespace G_Searcher.Models.Exceptions;

public abstract class StatusCodeException(ResponseError error) : Exception
{
    public abstract HttpStatusCode StatusCode { get; }
    public ResponseError Error { get; } = error;
}
