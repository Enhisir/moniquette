namespace Moniquette.Common.Exceptions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
    {
        return source.Where(item => item is not null).Select(item => item!);
    }
}