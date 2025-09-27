namespace Moniquette.Common.Utils;

public static class BaseUtils
{
    public static T? SafeGet<T>(Func<T> action, T? defaultValue = default)
    {
        try { return action(); } catch { return defaultValue; }
    }
}