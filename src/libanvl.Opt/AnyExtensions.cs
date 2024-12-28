using libanvl;

namespace libanvl;

/// <summary>
/// Provides extension methods for the <see cref="Any{T}"/> struct.
/// </summary>
public static class AnyExtensions
{
    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="source">A sequence of values to invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>An <see cref="Any{TResult}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
    public static Any<TResult> Select<T, TResult>(this Any<T> source, Func<T, TResult> selector) where T : notnull where TResult : notnull
    {
        if (source.IsNone)
        {
            return [];
        }

        if (source.IsSingle)
        {
            return new Any<TResult>(selector(source.Single.Unwrap()));
        }

        return [.. source.Many.Unwrap().Select(selector)];
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values to filter.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>An <see cref="Any{T}"/> that contains elements from the input sequence that satisfy the condition.</returns>
    public static Any<T> Where<T>(this Any<T> source, Func<T, bool> predicate) where T : notnull
    {
        if (source.IsNone)
        {
            return source;
        }

        if (source.IsSingle)
        {
            return predicate(source.Single.Unwrap()) ? source : [];
        }

        return [.. source.Many.Unwrap().Where(predicate)];
    }

    /// <summary>
    /// Projects each element of a sequence to an <see cref="IEnumerable{TResult}"/> and flattens the resulting sequences into one sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
    /// <param name="source">A sequence of values to project.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>An <see cref="Any{TResult}"/> whose elements are the result of invoking the one-to-many transform function on each element of the input sequence.</returns>
    public static Any<TResult> SelectMany<T, TResult>(this Any<T> source, Func<T, IEnumerable<TResult>> selector) where T : notnull where TResult : notnull
    {
        if (source.IsNone)
        {
            return [];
        }

        if (source.IsSingle)
        {
            return [.. selector(source.Single.Unwrap())];
        }

        return [.. source.Many.Unwrap().SelectMany(selector)];
    }

    /// <summary>
    /// Applies an accumulator function over a sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the accumulator value.</typeparam>
    /// <param name="source">A sequence to aggregate over.</param>
    /// <param name="seed">The initial accumulator value.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <returns>The final accumulator value.</returns>
    public static TResult Aggregate<T, TResult>(this Any<T> source, TResult seed, Func<TResult, T, TResult> func) where T : notnull
    {
        if (source.IsNone)
        {
            return seed;
        }

        if (source.IsSingle)
        {
            return func(seed, source.Single.Unwrap());
        }

        return source.Many.Unwrap().Aggregate(seed, func);
    }

    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence whose elements to apply the predicate to.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns><see langword="true"/> if any elements in the source sequence pass the test in the specified predicate; otherwise, <see langword="false"/>.</returns>
    public static bool Any<T>(this Any<T> source, Func<T, bool> predicate) where T : notnull
    {
        if (source.IsNone)
        {
            return false;
        }

        if (source.IsSingle)
        {
            return predicate(source.Single.Unwrap());
        }

        return source.Many.Unwrap().Any(predicate);
    }

    /// <summary>
    /// Determines whether all elements of a sequence satisfy a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence whose elements to apply the predicate to.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns><see langword="true"/> if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, <see langword="false"/>.</returns>
    public static bool All<T>(this Any<T> source, Func<T, bool> predicate) where T : notnull
    {
        if (source.IsNone)
        {
            return true;
        }

        if (source.IsSingle)
        {
            return predicate(source.Single.Unwrap());
        }

        return source.Many.Unwrap().All(predicate);
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence to return an element from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The first element in the sequence that passes the test in the specified predicate function, or a default value if no such element is found.</returns>
    public static T? FirstOrDefault<T>(this Any<T> source, Func<T, bool> predicate) where T : notnull
    {
        if (source.IsNone)
        {
            return default;
        }

        if (source.IsSingle)
        {
            return predicate(source.Single.Unwrap()) ? source.Single.Unwrap() : default;
        }

        return source.Many.Unwrap().FirstOrDefault(predicate);
    }

    /// <summary>
    /// Matches the state of the <see cref="Any{T}"/> instance and executes the corresponding action.
    /// </summary>
    /// <typeparam name="T">The type of the element.</typeparam>
    /// <param name="source">The <see cref="Any{T}"/> instance to match.</param>
    /// <param name="some">The action to execute if the instance contains an element.</param>
    /// <param name="none">The action to execute if the instance is empty.</param>
    public static void Match<T>(this Any<T> source, Action<T> some, Action none) where T : notnull
    {
        if (source.IsNone)
        {
            none();
        }
        else
        {
            some(source.Single.Unwrap());
        }
    }

    /// <summary>
    /// Creates a <see cref="Dictionary{TKey, TValue}"/> from an <see cref="Any{T}"/> according to a specified key selector function.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">A sequence to create a <see cref="Dictionary{TKey, TValue}"/> from.</param>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <returns>A <see cref="Dictionary{TKey, TValue}"/> that contains keys and values.</returns>
    public static Dictionary<TKey, T> ToDictionary<T, TKey>(this Any<T> source, Func<T, TKey> keySelector) where T : notnull where TKey : notnull
    {
        if (source.IsNone)
        {
            return [];
        }

        if (source.IsSingle)
        {
            return new Dictionary<TKey, T> { { keySelector(source.Single.Unwrap()), source.Single.Unwrap() } };
        }

        return source.Many.Unwrap().ToDictionary(keySelector);
    }
}
