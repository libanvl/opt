namespace libanvl.Exceptions;

/// <summary>
/// Represents an exception specific to the Opt library.
/// </summary>
/// <param name="code">The specific code representing the exception.</param>
public class OptException(OptException.OptExceptionCode code)
    : InvalidOperationException(GetMessage(code))
{
    /// <summary>
    /// Enumeration of possible OptException codes.
    /// </summary>
    public enum OptExceptionCode
    {
        /// <summary>
        /// Indicates that an <see cref="Opt" /> was initialized with a null value.
        /// </summary>
        InitializedWithNull,

        /// <summary>
        /// Indicates that an operation was attempted on an option that is None.
        /// </summary>
        OptIsNone,

        /// <summary>
        /// Indicates an internal error within the Opt library.
        /// </summary>
        InternalError
    }

    /// <summary>
    /// Gets the code representing the specific exception.
    /// </summary>
    public OptExceptionCode Code { get; } = code;

    /// <summary>
    /// Throws an OptException if the provided value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check for null.</param>
    /// <exception cref="OptException">Thrown when the value is null.</exception>
    public static void ThrowIfNull<T>(T? value)
    {
        _ = value ?? throw new OptException(OptExceptionCode.InitializedWithNull);
    }

    /// <summary>
    /// Throws an OptException with an internal error code if the provided value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check for null.</param>
    /// <returns>The provided value if it is not null.</returns>
    /// <exception cref="OptException">Thrown when the value is null.</exception>
    public static T ThrowInternalErrorIfNull<T>(T? value)
    {
        return value ?? throw new OptException(OptExceptionCode.InternalError);
    }

    /// <summary>
    /// Throws an OptException with an OptIsNone code if the provided value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check for null.</param>
    /// <returns>The provided value if it is not null.</returns>
    /// <exception cref="OptException">Thrown when the value is null.</exception>
    public static T ThrowOptIsNoneIfNull<T>(T? value)
    {
        return value ?? throw new OptException(OptExceptionCode.OptIsNone);
    }

    /// <summary>
    /// Retrieves the message associated with the specified exception code.
    /// </summary>
    /// <param name="code">The exception code.</param>
    /// <returns>The message associated with the exception code.</returns>
    private static string? GetMessage(OptExceptionCode code)
    {
        return code switch
        {
            OptExceptionCode.InitializedWithNull => "An Opt was initialized with a null value.",
            OptExceptionCode.OptIsNone => "An operation was attempted on an option that is None.",
            OptExceptionCode.InternalError => "An internal error occurred within the Opt library.",
            _ => null
        };
    }
}
