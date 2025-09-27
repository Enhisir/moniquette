using Moniquette.Common.Api.Result;

namespace Moniquette.Common.Api;

public class ApiError : IApiResult
{
    public bool Success => false;
    
    public HttpError Error { get; }
    
    protected internal ApiError(HttpError error)
    {
        Error = error;
    }
}