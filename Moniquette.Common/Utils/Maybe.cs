namespace Moniquette.Common.Utils;

public readonly struct Maybe<T>
{
    private readonly T? _value;
    private readonly string? _error;

    private bool IsSuccess { get; }
    
    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException($"Cannot access Value. Error: {_error}");
            
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)IsSuccess;
            }
            
            return _value!;
        }
    }
    
    public string Error => !IsSuccess ? _error! : throw new InvalidOperationException("No error when successful");
    
    private Maybe(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        _value = value;
        _error = error;
    }
    
    public static Maybe<T> Success(T value) => new(true, value, null);
    public static Maybe<T> Failure(string error) => new(false, default, error);
    
    public static Maybe<bool> Success() => new(true, true, null);
    
    public static implicit operator bool(Maybe<T> maybe) => maybe.IsSuccess;
}
