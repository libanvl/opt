using System.Collections;

namespace libanvl;

/// <summary>
/// Represents a container that can hold either a single instance of <typeparamref name="T"/> or multiple instances.
/// </summary>
/// <typeparam name="T">The type of elements in the container.</typeparam>
public struct OneOrMany<T> : IEquatable<OneOrMany<T>> where T : notnull
{
    private T? _single;
    private List<T>? _many;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOrMany{T}"/> struct containing a single element.
    /// </summary>
    /// <param name="single">The single element to be contained.</param>
    public OneOrMany(T single)
    {
        _single = single;
        _many = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOrMany{T}"/> struct containing multiple elements.
    /// </summary>
    /// <param name="many">The collection of elements to be contained.</param>
    public OneOrMany(IEnumerable<T> many)
    {
        _single = default;
        _many = [.. many];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOrMany{T}"/> struct containing multiple elements.
    /// </summary>
    /// <param name="many">The list of elements to be contained.</param>
    public OneOrMany(List<T> many)
    {
        _single = default;
        _many = many;
    }

    /// <summary>
    /// Gets a value indicating whether the container holds a single element.
    /// </summary>
    public readonly bool IsSingle => _many is null;

    /// <summary>
    /// Gets a value indicating whether the container holds multiple elements.
    /// </summary>
    public readonly bool IsMany => _many is not null;

    /// <summary>
    /// Gets the number of elements contained in the container.
    /// </summary>
    public readonly int Count => IsSingle ? 1 : _many!.Count;

    /// <summary>
    /// Gets the single element contained in the container.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the container does not hold a single element.</exception>
    public readonly T Single => IsSingle ? _single! : throw new InvalidOperationException("Not a single instance.");

    /// <summary>
    /// Gets the collection of elements contained in the container.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the container does not hold multiple elements.</exception>
    public readonly IReadOnlyList<T> Many => IsMany ? _many! : throw new InvalidOperationException("Not multiple instances.");

    /// <summary>
    /// Adds an element to the container.
    /// </summary>
    /// <param name="item">The element to add.</param>
    public void Add(T item)
    {
        if (IsSingle)
        {
            _many = [_single!, item];
            _single = default;
        }
        else
        {
            _many!.Add(item);
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
            return false;
        }
        else
        {
            bool removed = _many!.Remove(item);
            if (_many.Count == 1)
            {
                _single = _many[0];
                _many = null;
            }
            return removed;
        }
    }

    /// <summary>
    /// Determines whether the container contains a specific element.
    /// </summary>
    /// <param name="item">The element to locate.</param>
    /// <returns><see langword="true" /> if the element is found; otherwise, <see langword="false" />.</returns>
    public readonly bool Contains(T item) => IsSingle ? EqualityComparer<T>.Default.Equals(_single, item) : _many!.Contains(item);

    /// <summary>
    /// Converts the container to an array.
    /// </summary>
    /// <returns>An array containing the elements in the container.</returns>
    public readonly T[] ToArray() => IsSingle ? [_single!] : [.. _many!];

    /// <summary>
    /// Converts the container to a list.
    /// </summary>
    /// <returns>A list containing the elements in the container.</returns>
    public readonly List<T> ToList() => IsSingle ? [_single!] : _many!;

    /// <summary>
    /// Converts the container to an enumerable.
    /// </summary>
    /// <returns>An enumerable containing the elements in the container.</returns>
    public readonly IEnumerable<T> ToEnumerable() => IsSingle ? new[] { _single! } : _many!;

    /// <summary>
    /// Returns an enumerator that iterates through the elements in the container.
    /// </summary>
    /// <returns>An enumerator for the elements in the container.</returns>
    public readonly Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Implicitly converts a single element to a <see cref="OneOrMany{T}"/>.
    /// </summary>
    /// <param name="single">The single element to be converted.</param>
    public static implicit operator OneOrMany<T>(T single) => new(single);

    /// <summary>
    /// Implicitly converts a list of elements to a <see cref="OneOrMany{T}"/>.
    /// </summary>
    /// <param name="many">The list of elements to be converted.</param>
    public static implicit operator OneOrMany<T>(List<T> many) => new(many);

    /// <summary>
    /// Implicitly converts a <see cref="OneOrMany{T}"/> to a single element.
    /// </summary>
    /// <param name="oneOrMany">The <see cref="OneOrMany{T}"/> to be converted.</param>
    /// <exception cref="InvalidOperationException">Thrown if the container does not hold a single element.</exception>
    public static implicit operator T(OneOrMany<T> oneOrMany) => oneOrMany.Single;

    /// <summary>
    /// Implicitly converts a <see cref="OneOrMany{T}"/> to a list of elements.
    /// </summary>
    /// <param name="oneOrMany">The <see cref="OneOrMany{T}"/> to be converted.</param>
    public static implicit operator List<T>(OneOrMany<T> oneOrMany) => oneOrMany.ToList();

    /// <summary>
    /// Determines whether two <see cref="OneOrMany{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(OneOrMany<T> left, OneOrMany<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="OneOrMany{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(OneOrMany<T> left, OneOrMany<T> right) => !(left == right);

    /// <summary>
    /// Determines whether the current instance is equal to another instance of the same type.
    /// </summary>
    /// <param name="other">An instance to compare with this instance.</param>
    /// <returns><see langword="true"/> if the current instance is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
    public readonly bool Equals(OneOrMany<T> other)
    {
        if (IsSingle && other.IsSingle)
        {
            return EqualityComparer<T>.Default.Equals(_single, other._single);
        }
        if (IsMany && other.IsMany)
        {
            return _many!.SequenceEqual(other._many!);
        }
        return false;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the specified object is equal to the current instance; otherwise, <see langword="false"/>.</returns>
    public override readonly bool Equals(object? obj) => obj is OneOrMany<T> other && Equals(other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override readonly int GetHashCode()
    {
        if (IsSingle)
        {
            return _single!.GetHashCode();
        }

        if (IsMany)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(_many!);
        }

        return 0;
    }

    /// <summary>
    /// Custom enumerator for <see cref="OneOrMany{T}"/>.
    /// </summary>
    /// <param name="source">The <see cref="OneOrMany{T}"/> instance to enumerate.</param>
    public struct Enumerator(OneOrMany<T> source) : IEnumerator<T>
    {
        private int _index = -1;

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public readonly T Current => source.IsSingle
            ? source._single!
            : source._many![_index];

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
            if (source.IsSingle)
            {
                return ++_index == 0;
            }
            return ++_index < source._many!.Count;
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
