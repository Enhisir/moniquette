namespace Moniquette.Server.Auth;

public readonly record struct JwtValidationResult(
    bool IsValid,
    Guid SessionId,
    string? Error)
{
    public static JwtValidationResult Success(Guid sessionId)
        => new(true, sessionId, null);

    public static JwtValidationResult Fail(string error)
        => new(false, Guid.Empty, error);
}
