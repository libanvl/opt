namespace libanvl;

/// <summary>
/// Provides extension methods for working with <see cref="Opt{T}"/> instances.
/// </summary>
public static class OptExtensions
{
    /// <summary>
    /// Converts an enumerable of nullable reference types to an enumerable of <see cref="Opt{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source enumerable.</typeparam>
    /// <param name="source">The source enumerable of nullable reference types.</param>
    /// <returns>An enumerable of <see cref="Opt{T}"/> instances.</returns>
    public static IEnumerable<Opt<T>> AsOpts<T>(this IEnumerable<T?> source) where T : class
    {
        foreach (var item in source)
        {
            yield return item is null ? Opt<T>.None : Opt<T>.Some(item);
        }
    }

    /// <summary>
    /// Converts an enumerable of nullable value types to an enumerable of <see cref="Opt{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source enumerable.</typeparam>
    /// <param name="source">The source enumerable of nullable value types.</param>
    /// <returns>An enumerable of <see cref="Opt{T}"/> instances.</returns>
    public static IEnumerable<Opt<T>> AsOpts<T>(this IEnumerable<T?> source) where T : struct
    {
        foreach (var item in source)
        {
            yield return item.HasValue ? Opt<T>.Some(item.Value) : Opt<T>.None;
        }
    }

    /// <summary>
    /// Converts an enumerable of <see cref="Opt{T}"/> instances to an enumerable of the underlying values.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source enumerable.</typeparam>
    /// <param name="source">The source enumerable of <see cref="Opt{T}"/> instances.</param>
    /// <returns>An enumerable of the underlying values.</returns>
    public static IEnumerable<T> AsEnumerable<T>(this IEnumerable<Opt<T>> source) where T : notnull
    {
        foreach (var item in source)
        {
            if (item.IsSome)
            {
                yield return item.Unwrap();
            }
        }
    }
}
