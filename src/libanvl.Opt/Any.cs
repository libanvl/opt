using System.Collections;
using System.Runtime.CompilerServices;

namespace libanvl;

/// <summary>
/// Provides static methods to create instances of the <see cref="Any{T}"/> struct.
/// </summary>
[CollectionBuilder(typeof(Any), nameof(Create))]
public static class Any
{
    /// <summary>
    /// Creates a new <see cref="Any{T}"/> containing a single element.
    /// </summary>
    /// <typeparam name="T">The type of the element.</typeparam>
    /// <param name="single">The single element to contain.</param>
    /// <returns>A <see cref="Any{T}"/> with the specified element.</returns>
    public static Any<T> From<T>(T single) where T : notnull => new(single);

    /// <summary>
    /// Creates a new <see cref="Any{T}"/> containing multiple elements from a list.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="many">The list of elements to contain.</param>
    /// <returns>A <see cref="Any{T}"/> with the specified elements.</returns>
    public static Any<T> From<T>(List<T> many) where T : notnull => new(many);

    /// <summary>
    /// Creates a new <see cref="Any{T}"/> containing multiple elements from an enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="many">The enumerable collection of elements to contain.</param>
    /// <returns>A <see cref="Any{T}"/> with the specified elements.</returns>
    public static Any<T> From<T>(IEnumerable<T> many) where T : notnull => new(many);

    /// <summary>
    /// Creates a new <see cref="Any{T}"/> containing multiple elements from parameters.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="many">An array of elements to contain.</param>
    /// <returns>A <see cref="Any{T}"/> with the specified elements.</returns>
    public static Any<T> From<T>(params T[] many) where T : notnull => new(many.AsSpan());

    /// <summary>
    /// Creates a new <see cref="Any{T}"/> containing multiple elements from a read-only span.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="values">A read-only span of elements to contain.</param>
    /// <returns>A <see cref="Any{T}"/> with the specified elements.</returns>
    internal static Any<T> Create<T>(ReadOnlySpan<T> values) where T : notnull => new(values);
}

/// <summary>
/// Represents a container that can hold either a single instance of <typeparamref name="T"/> or multiple instances.
/// </summary>
/// <typeparam name="T">The type of elements in the container.</typeparam>
public struct Any<T> : IEquatable<Any<T>>, IEnumerable<T>
    where T : notnull
{
    private static readonly Any<T> none = [];

    private Opt<T> _single;
    private Opt<List<T>> _many;

    /// <summary>
    /// Initializes a new instance of the <see cref="Any{T}"/> struct with no elements.
    /// </summary>
    public Any()
    {
        _single = Opt<T>.None;
        _many = Opt<List<T>>.None;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Any{T}"/> struct containing a single element.
    /// </summary>
    /// <param name="single">The single element to be contained.</param>
    public Any(T single)
    {
        _single = single;
        _many = Opt<List<T>>.None;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Any{T}"/> struct containing multiple elements.
    /// </summary>
    /// <param name="many">The collection of elements.</param>
    public Any(IEnumerable<T>? many)
    {
        if (many is null)
        {
            _single = Opt<T>.None;
            _many = Opt<List<T>>.None;
        }
        else if (many.TryGetNonEnumeratedCount(out var count))
        {
            switch (count)
            {
                case 0:
                    _single = Opt<T>.None;
                    _many = Opt<List<T>>.None;
                    break;
                case 1:
                    _single = many.First();
                    _many = Opt<List<T>>.None;
                    break;
                default:
                    _single = Opt<T>.None;
                    _many = new List<T>(many);
                    break;
            }
        }
        else
        {
            _single = Opt<T>.None;
            _many = Opt<List<T>>.None;
            foreach (var item in many)
            {
                Add(item);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Any{T}"/> struct containing multiple elements.
    /// </summary>
    /// <param name="many">The list of elements.</param>
    public Any(List<T> many)
    {
        ArgumentNullException.ThrowIfNull(many);

        switch (many.Count)
        {
            case 0:
                _single = Opt<T>.None;
                _many = Opt<List<T>>.None;
                break;
            case 1:
                _single = many[0];
                _many = Opt<List<T>>.None;
                break;
            default:
                _single = Opt<T>.None;
                _many = many;
                break;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Any{T}"/> struct containing multiple elements.
    /// </summary>
    /// <param name="values">The list of value.</param>
    public Any(in ReadOnlySpan<T> values)
    {
        switch (values.Length)
        {
            case 0:
                _single = Opt<T>.None;
                _many = Opt<List<T>>.None;
                break;
            case 1:
                _single = values[0];
                _many = Opt<List<T>>.None;
                break;
            default:
                _single = Opt<T>.None;
                _many = Opt.From<List<T>>([.. values]);
                break;
        }
    }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <summary>
    /// Gets the empty container.
    /// </summary>
    public static Any<T> None => none;
#pragma warning restore CA1000 // Do not declare static members on generic types

    /// <summary>
    /// Gets a value indicating whether the container holds no elements.
    /// </summary>
    public readonly bool IsNone => _single.IsNone && _many.IsNone;

    /// <summary>
    /// Gets a value indicating whether the container holds any elements.
    /// </summary>
    public readonly bool IsSome => _single.IsSome || _many.IsSome;

    /// <summary>
    /// Gets a value indicating whether the container holds a single element.
    /// </summary>
    public readonly bool IsSingle
    {
        get
        {
            Verify.Operation(!(_single.IsSome && _many.IsSome));
            return _single.IsSome;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the container holds multiple elements.
    /// </summary>
    public readonly bool IsMany
    {
        get
        {
            Verify.Operation(!(_single.IsSome && _many.IsSome));
            return _many.IsSome;
        }
    }

    /// <summary>
    /// Gets the number of elements contained in the container.
    /// </summary>
    public readonly int Count => IsNone
        ? 0
        : IsSingle
            ? 1
            : _many.Select(x => x.Count);

    /// <summary>
    /// Gets the single element contained in the container.
    /// </summary>
    public readonly Opt<T> Single => _single;

    /// <summary>
    /// Gets the multiple elements contained in the container.
    /// </summary>
    public readonly Opt<IReadOnlyList<T>> Many => _many.Cast<IReadOnlyList<T>>();

    /// <summary>
    /// Adds an element to the container.
    /// </summary>
    /// <param name="item">The element to add.</param>
    public void Add(T item)
    {
        if (IsNone)
        {
            _single = item;
        }
        else if (IsSingle)
        {

            _many = new List<T> { _single.Unwrap(), item };
            _single = Opt<T>.None;
        }
        else
        {
            _many.Unwrap().Add(item);
        }
    }

    /// <summary>
    /// Removes an element from the container.
    /// </summary>
    /// <param name="item">The element to remove.</param>
    /// <returns><see langword="true" /> if the element was successfully removed; otherwise, <see langword="false" />.</returns>
    public bool Remove(T item)
    {
        if (IsSingle)
        {
            if (EqualityComparer<T>.Default.Equals(_single.Unwrap(), item))
            {
                _single = Opt<T>.None;
                return true;
            }
        }
        else if (IsMany)
        {
            if (_many.Unwrap().Remove(item))
            {
                if (_many.Unwrap().Count == 1)
                {
                    _single = _many.Unwrap()[0];
                    _many = Opt<List<T>>.None;
                }
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the container contains a specific element.
    /// </summary>
    /// <param name="item">The element to locate.</param>
    /// <returns><see langword="true" /> if the element is found; otherwise, <see langword="false" />.</returns>
    public readonly bool Contains(T item) => IsSingle
        ? EqualityComparer<T>.Default.Equals(_single.Unwrap(), item)
        : IsMany && _many.Unwrap().Contains(item);

    /// <summary>
    /// Converts the container to an array.
    /// </summary>
    /// <returns>An array containing the elements in the container.</returns>
    public readonly T[] ToArray() => IsNone
        ? []
        : IsSingle
            ? [_single.Unwrap()]
            : [.. _many.Unwrap()];

    /// <summary>
    /// Converts the container to a list.
    /// </summary>
    /// <returns>A list containing the elements in the container.</returns>
    public readonly List<T> ToList() => IsNone
        ? new()
        : IsSingle
            ? new() { _single.Unwrap() }
            : new(_many.Unwrap());

    /// <summary>
    /// Converts the container to an enumerable.
    /// </summary>
    /// <returns>An enumerable containing the elements in the container.</returns>
    public readonly IEnumerable<T> ToEnumerable() => IsNone
        ? Array.Empty<T>()
        : IsSingle
            ? new T[] { _single.Unwrap() }
            : _many.Unwrap();

    /// <summary>
    /// Returns an enumerator that iterates through the elements in the container.
    /// </summary>
    /// <returns>An enumerator for the elements in the container.</returns>
    public readonly Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Implicitly converts a single element to a <see cref="Any{T}"/>.
    /// </summary>
    /// <param name="single">The single element to be converted.</param>
    public static implicit operator Any<T>(T single) => new(single);

    /// <summary>
    /// Implicitly converts a list of elements to a <see cref="Any{T}"/>.
    /// </summary>
    /// <param name="many">The list of elements to be converted.</param>
    public static implicit operator Any<T>(List<T> many) => [.. many];

    /// <summary>
    /// Implicitly converts a <see cref="Any{T}"/> to a single element.
    /// </summary>
    /// <param name="Any">The <see cref="Any{T}"/> to be converted.</param>
    /// <exception cref="InvalidOperationException">Thrown if the container does not hold a single element.</exception>
    public static implicit operator T?(in Any<T> Any) => Any.Single;

    /// <summary>
    /// Implicitly converts a <see cref="Any{T}"/> to a list of elements.
    /// </summary>
    /// <param name="Any">The <see cref="Any{T}"/> to be converted.</param>
    public static implicit operator List<T>(in Any<T> Any) => [.. Any];

    /// <summary>
    /// Determines whether two <see cref="Any{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(in Any<T> left, in Any<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Any{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(in Any<T> left, in Any<T> right) => !(left == right);

    /// <summary>
    /// Converts the container to an optional value.
    /// </summary>
    public readonly Opt<IReadOnlyList<T>> ToOpt() => IsNone
        ? Opt<IReadOnlyList<T>>.None
        : IsSingle
            ? Opt.From<IReadOnlyList<T>>([_single.Unwrap()])
            : Opt.From<IReadOnlyList<T>>(_many.Unwrap());

    /// <summary>
    /// Casts the elements of the container to the specified type.
    /// </summary>
    /// <typeparam name="U">The type to cast the elements to.</typeparam>
    /// <returns>A new <see cref="Any{U}"/> containing the casted elements.</returns>
    public readonly Any<U> Cast<U>() where U : notnull => IsNone
        ? Any<U>.None
        : IsSingle
            ? Any.From((U)(object)_single.Unwrap())
            : Any.From(_many.Unwrap().Cast<U>());

    /// <summary>
    /// Determines whether the current instance is equal to another instance of the same type.
    /// </summary>
    /// <param name="other">An instance to compare with this instance.</param>
    /// <returns><see langword="true"/> if the current instance is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
    public readonly bool Equals(in Any<T> other)
    {
        if (IsNone && other.IsNone)
        {
            return true;
        }

        if (IsSingle && other.IsSingle)
        {
            return EqualityComparer<T>.Default.Equals(_single, other._single);
        }

        if (IsMany && other.IsMany)
        {
            return _many.Unwrap().SequenceEqual(other._many.Unwrap());
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the specified object is equal to the current instance; otherwise, <see langword="false"/>.</returns>
    public override readonly bool Equals(object? obj) => obj is Any<T> other && Equals(other);

    readonly bool IEquatable<Any<T>>.Equals(Any<T> other) => Equals(in other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override readonly int GetHashCode()
    {
        if (IsSingle)
        {
            return _single.Unwrap().GetHashCode();
        }

        if (IsMany)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(_many.Unwrap());
        }

        return 0;
    }

    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Custom enumerator for <see cref="Any{T}"/>.
    /// </summary>
    /// <param name="source">The <see cref="Any{T}"/> instance to enumerate.</param>
    public struct Enumerator(Any<T> source) : IEnumerator<T>
    {
        private int _index = -1;

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public readonly T Current => source.IsSingle
            ? source._single!
            : source._many.Unwrap()[_index];

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        readonly object IEnumerator.Current => Current;

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            if (source.IsNone)
            {
                return false;
            }

            return source.IsSingle
                ? ++_index == 0
                : ++_index < source._many.Unwrap().Count;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset() => _index = -1;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public readonly void Dispose() { }
    }
}
