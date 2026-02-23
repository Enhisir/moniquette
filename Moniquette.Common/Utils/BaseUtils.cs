namespace Moniquette.Common.Utils;

public static class BaseUtils
{
    public static T? TryInvoke<T>(Func<T> func, T? defaultValue = default)
    {
        try { return func(); } catch { return defaultValue; }
    }
    
    public static void TryInvoke<TException>(
        Action action,
        Action<TException>? exceptionAction = null)
        where TException : Exception
    {
        try
        {
            action.Invoke();
        }
        catch (TException e)
        {
            exceptionAction?.Invoke(e);
        }
    }
}