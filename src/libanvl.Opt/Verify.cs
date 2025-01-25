using System.Runtime.CompilerServices;

namespace libanvl;

/// <summary>
/// Provides methods to verify conditions and throw exceptions if the conditions are not met.
/// </summary>
public static class Verify
{
    /// <summary>
    /// Verifies that a condition is true. If the condition is false, throws an <see cref="InvalidOperationException"/> with the specified message.
    /// </summary>
    /// <param name="condition">The condition to verify.</param>
    /// <param name="message">The message to include in the exception if the condition is false.</param>
    /// <exception cref="InvalidOperationException">Thrown when the condition is false.</exception>
    public static void Operation(bool condition, FormattableString message) =>
        Operation(condition, message.ToString());

#if NET8_0_OR_GREATER
    /// <summary>
    /// Verifies that a condition is true. If the condition is false, throws an <see cref="InvalidOperationException"/> with the specified message.
    /// </summary>
    /// <param name="condition">The condition to verify.</param>
    /// <param name="message">The message to include in the exception if the condition is false.</param>
    /// <exception cref="InvalidOperationException">Thrown when the condition is false.</exception>
    public static void Operation(bool condition, [CallerArgumentExpression(nameof(condition))] string? message = null)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
#else
    /// <summary>
    /// Verifies that a condition is true. If the condition is false, throws an <see cref="InvalidOperationException"/> with the specified message.
    /// </summary>
    /// <param name="condition">The condition to verify.</param>
    /// <param name="message">The message to include in the exception if the condition is false.</param>
    /// <exception cref="InvalidOperationException">Thrown when the condition is false.</exception>
    public static void Operation(bool condition, string? message = null)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
#endif
}
