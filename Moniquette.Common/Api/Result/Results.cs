using Moniquette.Common.Api.Result;

namespace Moniquette.Common.Api;

public static class Results
{
    public static IApiResult Ok() => new ApiOk<object>();
    public static IApiResult Ok<TResponseData>(TResponseData value) where TResponseData: class => new ApiOk<TResponseData>(value);
    public static IApiResult Error(HttpError error) => new ApiError(error);
}