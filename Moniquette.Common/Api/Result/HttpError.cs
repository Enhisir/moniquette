using System.Net;

namespace Moniquette.Common.Api.Result;

public class HttpError
{
    public HttpStatusCode ErrorCode { get; init; }
    public string? Message { get; init; }
};