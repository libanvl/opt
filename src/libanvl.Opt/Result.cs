using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace libanvl;

/// <summary>
/// Provides static methods for creating and working with <see cref="Result{TResult, TError}"/> instances.
/// </summary>
public static class Result
{
    /// <summary>
    /// A delegate representing a validator function that returns an error value if the validation fails.
    /// </summary>
    public delegate bool Validator<TValue, TError>(in TValue value, [NotNullWhen(false)] out TError? error)
        where TValue : notnull
        where TError : notnull;

    /// <summary>
    /// Creates a new <see cref="Result{TResult, TError}"/> representing a success.
    /// </summary>
    /// <typeparam name="TResult">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="value">The success value.</param>
    /// <returns>A <see cref="Result{TResult, TError}"/> representing a success.</returns>
    public static Result<TResult, TError> Ok<TResult, TError>(TResult value)
        where TResult : notnull
        where TError : notnull => new(value);

    /// <summary>
    /// Creates a new <see cref="Result{TResult, TError}"/> representing an error.
    /// </summary>
    /// <typeparam name="TResult">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="error">The error value.</param>
    /// <returns>A <see cref="Result{TResult, TError}"/> representing an error.</returns>
    public static Result<TResult, TError> Err<TResult, TError>(TError error)
        where TResult : notnull
        where TError : notnull => new(error);

    /// <summary>
    /// Creates a new <see cref="Result{TResult, TError}"/> from the specified success and error values.
    /// </summary>
    /// <typeparam name="TResult">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="value">The success value.</param>
    /// <param name="error">The error value.</param>
    /// <returns>A <see cref="Result{TResult, TError}"/> containing the specified success and error values.</returns>
    public static Result<TResult, TError> From<TResult, TError>(Opt<TResult> value, Opt<TError> error)
        where TResult : notnull
        where TError : notnull
    {
        return new(value, error);
    }

    /// <summary>
    /// Creates a new <see cref="Result{TResult, Exception}"/> representing a success.
    /// </summary>
    /// <typeparam name="TResult">The type of the success value.</typeparam>
    /// <param name="value">The success value.</param>
    /// <returns>A <see cref="Result{TResult, Exception}"/> representing a success.</returns>
    public static Result<TResult, Exception> Ok<TResult>(TResult value)
        where TResult : notnull => new(value);

    /// <summary>
    /// Creates a new <see cref="Result{TResult, Exception}"/> representing an error.
    /// </summary>
    /// <typeparam name="TResult">The type of the success value.</typeparam>
    /// <param name="error">The error value.</param>
    /// <returns>A <see cref="Result{TResult, Exception}"/> representing an error.</returns>
    public static Result<TResult, Exception> Err<TResult>(Exception error)
        where TResult : notnull => new(error);

    /// <summary>
    /// Executes the specified function and returns a <see cref="Result{TResult, Exception}"/> representing the outcome.
    /// </summary>
    /// <typeparam name="TResult">The type of the success value.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A <see cref="Result{TResult, Exception}"/> representing the outcome of the function execution.</returns>
    public static Result<TResult, Exception> Try<TResult>(Func<TResult> func)
        where TResult : notnull
    {
        try
        {
            return Ok(func());
        }
        catch (Exception ex)
        {
            return Err<TResult>(ex);
        }
    }

    /// <summary>
    /// Executes the specified function with the provided argument and returns a <see cref="Result{TResult, Exception}"/> representing the outcome.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument.</typeparam>
    /// <typeparam name="TResult">The type of the success value.</typeparam>
    /// <param name="arg">The argument to pass to the function.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A <see cref="Result{TResult, Exception}"/> representing the outcome of the function execution.</returns>
    public static Result<TResult, Exception> Try<TArg, TResult>(TArg arg, Func<TArg, TResult> func)
        where TResult : notnull
    {
        try
        {
            return Ok(func(arg));
        }
        catch (Exception ex)
        {
            return Err<TResult>(ex);
        }
    }

    /// <summary>
    /// Validates the specified value using the provided validators.
    /// </summary>
    /// <typeparam name="TResult">The type of the value to validate.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="validators">The validators to use for validation.</param>
    /// <returns>
    /// A <see cref="Result{TResult, TError}"/> representing the outcome of the validation.
    /// If validation succeeds, the result is a success containing the value.
    /// If validation fails, the result is an error containing the validation errors.
    /// </returns>
    public static Result<TResult, ImmutableArray<TError>> Validate<TResult, TError>(TResult value, params Validator<TResult, TError>[] validators)
        where TResult : notnull
        where TError : notnull
    {
        if (validators.Length == 0)
            return Ok<TResult, ImmutableArray<TError>>(value);

        var errors = ImmutableArray.CreateBuilder<TError>(initialCapacity: validators.Length);
        foreach (var validator in validators)
        {
            if (!validator(value, out TError? error))
                errors.Add(error);
        }

        if (errors.Count > 0)
            return Err<TResult, ImmutableArray<TError>>(errors.ToImmutable());

        return Ok<TResult, ImmutableArray<TError>>(value);
    }
}

/// <summary>
/// Represents a result that can either be a success (Ok) or an error (Err).
/// </summary>
/// <typeparam name="TResult">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
public readonly struct Result<TResult, TError>
    where TResult : notnull
    where TError : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TResult, TError}"/> struct with a success value.
    /// </summary>
    /// <param name="value">The success value.</param>
    public Result(TResult value)
    {
        Value = Opt.Some(value);
        Error = Opt<TError>.None;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TResult, TError}"/> struct with an error value.
    /// </summary>
    /// <param name="error">The error value.</param>
    public Result(TError error)
    {
        Value = Opt<TResult>.None;
        Error = Opt.Some(error);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TResult, TError}"/> struct with a success value.
    /// </summary>
    /// <param name="value">The success value.</param>
    public Result(Opt<TResult> value)
    {
        Value = value;
        Error = Opt<TError>.None;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TResult, TError}"/> struct with an error value.
    /// </summary>
    /// <param name="error">The error value.</param>
    public Result(Opt<TError> error)
    {
        Value = Opt<TResult>.None;
        Error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TResult, TError}"/> struct with a success value and an error value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <param name="error">The error value.</param>
    public Result(Opt<TResult> value, Opt<TError> error)
    {
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Deconstructs the result into its value and error components.
    /// </summary>
    /// <param name="value">The success value, if present.</param>
    /// <param name="error">The error value, if present.</param>
    public void Deconstruct(out Opt<TResult> value, out Opt<TError> error)
    {
        value = Value;
        error = Error;
    }

    /// <summary>
    /// Gets the success value, if present.
    /// </summary>
    public Opt<TResult> Value { get; }

    /// <summary>
    /// Gets the error value, if present.
    /// </summary>
    public Opt<TError> Error { get; }

    /// <summary>
    /// Gets a value indicating whether the result is a success.
    /// </summary>
    public bool IsOk => Value.IsSome;

    /// <summary>
    /// Gets a value indicating whether the result is an error.
    /// </summary>
    public bool IsErr => Error.IsSome;

    /// <summary>
    /// Unwraps the success value, if present.
    /// </summary>
    /// <returns>The success value.</returns>
    public TResult Unwrap()
    {
        if (IsOk)
            return Value.Unwrap();

        TError error = Error.Unwrap();
        if (error is Exception exception)
            throw exception;

        throw new InvalidOperationException(error.ToString());
    }

    /// <summary>
    /// Matches the result, invoking the appropriate action based on whether it is a success or an error.
    /// </summary>
    /// <param name="ok">The action to invoke if the result is a success.</param>
    /// <param name="err">The action to invoke if the result is an error.</param>
    public void Match(Action<TResult> ok, Action<TError> err)
    {
        if (IsOk)
            ok(Value.Unwrap());
        else
            err(Error.Unwrap());
    }

    /// <summary>
    /// Matches the result, invoking the appropriate function based on whether it is a success or an error.
    /// </summary>
    /// <typeparam name="T">The type of the result of the function.</typeparam>
    /// <param name="ok">The function to invoke if the result is a success.</param>
    /// <param name="err">The function to invoke if the result is an error.</param>
    /// <returns>The result of the invoked function.</returns>
    public T Match<T>(Func<TResult, T> ok, Func<TError, T> err)
    {
        if (IsOk)
            return ok(Value.Unwrap());
        else
            return err(Error.Unwrap());
    }

    /// <summary>
    /// Returns the success value if present, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the success value is not present.</param>
    /// <returns>The success value or the default value.</returns>
    public TResult OkOr(TResult defaultValue) => Value.SomeOr(defaultValue);

    /// <summary>
    /// Returns the success value if present, otherwise returns the result of the specified function.
    /// </summary>
    /// <param name="fn">The function to invoke if the success value is not present.</param>
    /// <returns>The success value or the result of the function.</returns>
    public TResult OkOr<T>(Func<TResult> fn) => Value.SomeOr(fn);

    /// <summary>
    /// Returns the success value if present, otherwise returns the default value of <typeparamref name="TResult"/>.
    /// </summary>
    /// <returns>The success value or the default value.</returns>
    public TResult? OkOrDefault() => Value.SomeOrDefault();

    /// <summary>
    /// Implicitly converts a success value to a <see cref="Result{TResult, TError}"/>.
    /// </summary>
    /// <param name="value">The success value.</param>
    public static implicit operator Result<TResult, TError>(TResult value) => new(value);

    /// <summary>
    /// Implicitly converts an error value to a <see cref="Result{TResult, TError}"/>.
    /// </summary>
    /// <param name="error">The error value.</param>
    public static implicit operator Result<TResult, TError>(TError error) => new(error);
}
