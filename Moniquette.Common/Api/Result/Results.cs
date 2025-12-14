namespace Moniquette.Common.Api.Result;

public static class Results
{
    public static IApiResult Ok() => new ApiOk<object>();
    public static IApiResult Ok<TResponseData>(TResponseData value) where TResponseData: class => new ApiOk<TResponseData>(value);
    public static IApiResult Error(HttpError error) => new ApiError(error.Message ?? "unknown error");
    public static IApiResult Error(string errorMessage) => new ApiError(errorMessage);
}