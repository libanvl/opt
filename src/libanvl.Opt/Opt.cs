using libanvl.Exceptions;
using System.Collections;

namespace libanvl;

/// <summary>
/// Provides static methods for creating and working with <see cref="Opt{T}"/> instances.
/// </summary>
public static class Opt
{
    /// <summary>
    /// Gets an <see cref="Opt{T}"/> representing the absence of a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>An <see cref="Opt{T}"/> representing the absence of a value.</returns>
    public static Opt<T> None<T>() where T : notnull => Opt<T>.None;

    /// <summary>
    /// Creates an <see cref="Opt{T}"/> with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to wrap.</param>
    /// <returns>An <see cref="Opt{T}"/> containing the value.</returns>
    public static Opt<T> Some<T>(T value) where T : notnull => Opt<T>.Some(value);

    /// <summary>
    /// Creates an <see cref="Opt{T}"/> from a nullable value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The nullable value to wrap.</param>
    /// <returns>An <see cref="Opt{T}"/> containing the value if not null, otherwise <see cref="Opt{T}.None"/>.</returns>
    public static Opt<T> From<T>(T? value) where T : notnull => Opt<T>.From(value);

    /// <summary>
    /// Combines two options into an option containing a tuple of their values if both are present.
    /// </summary>
    /// <typeparam name="T">The type of the first value.</typeparam>
    /// <typeparam name="U">The type of the second value.</typeparam>
    /// <param name="opt1">The first option.</param>
    /// <param name="opt2">The second option.</param>
    /// <returns>An option containing a tuple of the values if both are present, otherwise none.</returns>
    public static Opt<(T, U)> Zip<T, U>(Opt<T> opt1, Opt<U> opt2)
        where T : notnull
        where U : notnull
    {
        return opt1.IsSome && opt2.IsSome
            ? Opt<(T, U)>.Some((opt1.Unwrap(), opt2.Unwrap()))
            : Opt<(T, U)>.None;
    }

    /// <summary>
    /// Flattens an <see cref="Opt{Opt}"/> to an <see cref="Opt{T}"/>.
    /// </summary>
    /// <param name="opt">The option to flatten.</param>
    /// <returns>The flattened option.</returns>
    public static Opt<T> Flatten<T>(Opt<Opt<T>> opt)
        where T : notnull => opt.IsSome ? opt.Unwrap() : Opt<T>.None;
}

/// <summary>
/// Represents an optional value that may or may not be present.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public readonly struct Opt<T> : IEquatable<Opt<T>> where T : notnull
{
    /// <summary>
    /// Represents the absence of a value.
    /// </summary>
    public static readonly Opt<T> None = new();

    private readonly T? _value;

    private Opt(T value)
    {
        OptException.ThrowIfNull(value);
        _value = value;
    }

    /// <summary>
    /// Creates an <see cref="Opt{T}"/> with a value.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>An <see cref="Opt{T}"/> containing the value.</returns>
    public static Opt<T> Some(T value) => new(value);

    /// <summary>
    /// Creates an <see cref="Opt{T}"/> from a nullable value.
    /// </summary>
    /// <param name="value">The nullable value to wrap.</param>
    /// <returns>An <see cref="Opt{T}"/> containing the value if not null, otherwise <see cref="Opt{T}.None"/>.</returns>
    public static Opt<T> From(T? value) => value is null ? None : Some(value);

    /// <summary>
    /// Creates an <see cref="Opt{T}"/> from another <see cref="Opt{T}"/>.
    /// </summary>
    /// <param name="value">The option to wrap.</param>
    /// <returns>The same <see cref="Opt{T}"/> instance.</returns>
    public static Opt<T> From(Opt<T> value) => value;

    /// <inheritdoc/>
    public static bool operator ==(Opt<T> left, Opt<T> right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(Opt<T> left, Opt<T> right) => !left.Equals(right);

    /// <inheritdoc/>
    public static bool operator true(Opt<T> option) => option.IsSome;

    /// <inheritdoc/>
    public static bool operator false(Opt<T> option) => option.IsNone;

    /// <inheritdoc/>
    public static bool operator |(Opt<T> left, Opt<T> right) => left.IsSome || right.IsSome;

    /// <inheritdoc/>
    public static T operator |(Opt<T> option, T defaultValue) => option.SomeOr(defaultValue);

    /// <inheritdoc/>
    public static implicit operator Opt<T>(T? value) => value is null ? None : Some(value);

    /// <inheritdoc/>
    public static implicit operator T?(Opt<T> option) => option.IsSome ? option.Unwrap() : default;

    /// <inheritdoc/>
    public static implicit operator Opt<T>(Opt<Opt<T>> option) => Opt.Flatten(option);

    /// <summary>
    /// Deconstructs the <see cref="Opt{T}"/> into its value and a boolean indicating if it has a value.
    /// </summary>
    /// <param name="value">The value, if present.</param>
    /// <param name="isSome">True if the value is present, otherwise false.</param>
    public void Deconstruct(out T? value, out bool isSome)
    {
        value = _value;
        isSome = IsSome;
    }

    /// <summary>
    /// Gets a value indicating whether the option has a value.
    /// </summary>
    public bool IsSome => _value is not null;

    /// <summary>
    /// Gets a value indicating whether the option does not have a value.
    /// </summary>
    public bool IsNone => !IsSome;

    /// <summary>
    /// Unwraps the value if present, otherwise throws an exception.
    /// </summary>
    /// <returns>The value.</returns>
    /// <exception cref="OptException">Thrown if the value is not present.</exception>
    public T Unwrap() => OptException.ThrowOptIsNoneIfNull(_value);

    /// <summary>
    /// Returns the value if present, otherwise returns the specified fallback value.
    /// </summary>
    /// <param name="fallback">The fallback value to return if the value is not present.</param>
    /// <returns>The value or the default value.</returns>
    public T SomeOr(T fallback) => IsSome
        ? OptException.ThrowInternalErrorIfNull(_value)
        : fallback;

    /// <summary>
    /// Returns the value if present, otherwise returns the result of the specified function.
    /// </summary>
    /// <param name="fn">The function to invoke if the value is not present.</param>
    /// <returns>The value or the result of the function.</returns>
    public T SomeOr(Func<T> fn) => IsSome
        ? OptException.ThrowInternalErrorIfNull(_value)
        : fn();

    /// <summary>
    /// Returns the value if present, otherwise returns default value of <typeparamref name="T"/>.
    /// </summary>
    /// <returns>The value or <see langword="default"/>.</returns>
    public T? SomeOrDefault() => IsSome ? _value : default;

    /// <summary>
    /// Invokes one of the specified actions depending on whether the value is present.
    /// </summary>
    /// <param name="some">The action to invoke if the value is present.</param>
    /// <param name="none">The action to invoke if the value is not present.</param>
    public void Match(Action<T> some, Action none)
    {
        if (IsSome)
            some(OptException.ThrowInternalErrorIfNull(_value));
        else
            none();
    }

    /// <summary>
    /// Invokes one of the specified functions depending on whether the value is present.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="some">The function to invoke if the value is present.</param>
    /// <param name="none">The function to invoke if the value is not present.</param>
    /// <returns>The result of the invoked function.</returns>
    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) => IsSome
        ? some(OptException.ThrowInternalErrorIfNull(_value))
        : none();

    /// <summary>
    /// Transforms the value if present using the specified function.
    /// </summary>
    /// <typeparam name="U">The type of the result.</typeparam>
    /// <param name="fn">The function to transform the value.</param>
    /// <returns>An <see cref="Opt{U}"/> containing the transformed value or none.</returns>
    public Opt<U> Select<U>(Func<T, U> fn) where U : notnull => IsSome
        ? Opt<U>.Some(fn(OptException.ThrowInternalErrorIfNull(_value)))
        : Opt<U>.None;

    /// <summary>
    /// Casts the value to the specified type if present.
    /// </summary>
    /// <typeparam name="U">The type to cast to.</typeparam>
    /// <returns>An <see cref="Opt{U}"/> containing the casted value or none.</returns>
    public Opt<U> Cast<U>() where U : notnull => OptException.ThrowInternalErrorIfNull(_value) is U casted
        ? Opt<U>.Some(casted)
        : Opt<U>.None;

    /// <inheritdoc/>
    public bool Equals(Opt<T> other) => Equals(other, EqualityComparer<T>.Default);

    /// <inheritdoc/>
    public bool Equals(Opt<T> other, IEqualityComparer<T> comparer) => IsSome == other.IsSome && comparer.Equals(_value, other._value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Opt<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_value, IsSome);

    /// <inheritdoc/>
    public override string ToString() => IsSome ? $"Some({_value})" : "None";

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Gets an enumerator for the value if present.
    /// </summary>
    /// <param name="opt">The value</param>
    public struct Enumerator(Opt<T> opt) : IEnumerator<T>
    {
        private bool _hasValue = false;

        /// <inheritdoc/>
        public readonly T Current => opt.Unwrap();

        readonly object? IEnumerator.Current => Current;

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (_hasValue)
                return false;

            _hasValue = true;
            return opt.IsSome;
        }

        /// <inheritdoc/>
        public void Reset() => _hasValue = false;

        /// <inheritdoc/>
        public readonly void Dispose() { }
    }
}
