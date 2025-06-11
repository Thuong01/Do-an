
using Datas.Infrastructures;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq;

public static class EnumeratbleExtensions
{

    /// <summary>
    /// Used for paging. Can be used as an alternative to Skip(...).Take(...) chaining.
    /// </summary>
    public static IEnumerable<T> PageBy<T>([NotNull] this IEnumerable<T> query, int skipCount, int maxResultCount)
    {
        Check.NotNull(query, nameof(query));

        return query.Skip(skipCount).Take(maxResultCount);
    }

    /// <summary>
    /// Used for paging. Can be used as an alternative to Skip(...).Take(...) chaining.
    /// </summary>
    public static TEnumerable PageBy<T, TEnumerable>([NotNull] this TEnumerable query, int skipCount, int maxResultCount)
        where TEnumerable : IEnumerable<T>
    {
        Check.NotNull(query, nameof(query));

        return (TEnumerable)query.Skip(skipCount).Take(maxResultCount);
    }

    /// <summary>
    /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static IEnumerable<T> WhereIf<T>([NotNull] this IEnumerable<T> enumerable, bool condition, Func<T, bool> predicate)
    {
        Check.NotNull(enumerable, nameof(enumerable));

        return condition ? enumerable.Where(predicate) : enumerable;
    }

    /// <summary>
    /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static TEnumerable WhereIf<T, TEnumerable>([NotNull] this TEnumerable query, bool condition, Func<T, bool> predicate)
        where TEnumerable : IEnumerable<T>
    {
        Check.NotNull(query, nameof(query));

        return condition ? (TEnumerable)query.Where(predicate) : query;
    }

    /// <summary>
    /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static IEnumerable<T> WhereIf<T>([NotNull] this IEnumerable<T> query, bool condition, Func<T, int, bool> predicate)
    {
        Check.NotNull(query, nameof(query));

        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static TEnumerable WhereIf<T, TEnumerable>([NotNull] this TEnumerable query, bool condition, Func<T, int, bool> predicate)
        where TEnumerable : IEnumerable<T>
    {
        Check.NotNull(query, nameof(query));

        return condition ? (TEnumerable)query.Where(predicate) : query;
    }

    /// <summary>
    /// Order a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="sorting">Order the query</param>
    /// <returns>Order or not order query based on <paramref name="condition"/></returns>
    public static TEnumerable OrderByIf<T, TEnumerable>([NotNull] this TEnumerable enumerable, bool condition, Func<IEnumerable<T>, IOrderedEnumerable<T>> orderByFunc)
            where TEnumerable : IEnumerable<T>
    {
        Check.NotNull(enumerable, nameof(enumerable));

        return condition ? (TEnumerable)orderByFunc(enumerable) : enumerable;
    }

}

