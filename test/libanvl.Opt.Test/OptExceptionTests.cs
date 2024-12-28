using Xunit;
using libanvl.Exceptions;

namespace libanvl.opt.test;

public class OptExceptionTests
{
    [Fact]
    public void ThrowIfNull_ShouldThrowOptException_WhenValueIsNull()
    {
        Assert.Throws<OptException>(() => OptException.ThrowIfNull<object>(null));
    }

    [Fact]
    public void ThrowIfNull_ShouldNotThrow_WhenValueIsNotNull()
    {
        var value = new object();
        var exception = Record.Exception(() => OptException.ThrowIfNull(value));
        Assert.Null(exception);
    }

    [Fact]
    public void ThrowInternalErrorIfNull_ShouldThrowOptException_WhenValueIsNull()
    {
        Assert.Throws<OptException>(() => OptException.ThrowInternalErrorIfNull<object>(null));
    }

    [Fact]
    public void ThrowInternalErrorIfNull_ShouldNotThrow_WhenValueIsNotNull()
    {
        var value = new object();
        var exception = Record.Exception(() => OptException.ThrowInternalErrorIfNull(value));
        Assert.Null(exception);
    }

    [Fact]
    public void ThrowOptIsNoneIfNull_ShouldThrowOptException_WhenValueIsNull()
    {
        Assert.Throws<OptException>(() => OptException.ThrowOptIsNoneIfNull<object>(null));
    }

    [Fact]
    public void ThrowOptIsNoneIfNull_ShouldNotThrow_WhenValueIsNotNull()
    {
        var value = new object();
        var exception = Record.Exception(() => OptException.ThrowOptIsNoneIfNull(value));
        Assert.Null(exception);
    }

    [Fact]
    public void OptException_ShouldSetCodeProperty()
    {
        var exception = new OptException(OptException.OptExceptionCode.InternalError);
        Assert.Equal(OptException.OptExceptionCode.InternalError, exception.Code);
    }

    [Fact]
    public void GetMessage_ShouldReturnCorrectMessage()
    {
        var message = typeof(OptException)
            .GetMethod("GetMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?.Invoke(null, new object[] { OptException.OptExceptionCode.InitializedWithNull });

        Assert.Equal("An Opt was initialized with a null value.", message);
    }
}
