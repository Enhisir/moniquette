namespace Moniquette.Common.Api.Result;

public class ApiOk<TResponseData> : IApiResult where TResponseData : class
{
    public bool Success => true;

    public TResponseData? Value { get; }
    
    internal ApiOk(TResponseData? value = null)
    {
        Value = value;
    }
}