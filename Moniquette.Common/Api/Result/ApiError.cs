namespace Moniquette.Common.Api.Result;

public class ApiError : IApiResult
{
    public bool Success => false;
    
    public string ErrorMessage { get; }
    
    protected internal ApiError(string error)
    {
        ErrorMessage = error;
    }
    
    public override string ToString() => $"{typeof(ApiError).FullName}{Environment.NewLine}{ErrorMessage}";
}