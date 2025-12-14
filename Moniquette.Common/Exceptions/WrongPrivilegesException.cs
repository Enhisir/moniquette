namespace Moniquette.Common.Exceptions;

public class WrongPrivilegesException(string? message = null) : UnauthorizedAccessException(message);