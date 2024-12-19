using libanvl.Exceptions;
using System;
using Xunit;

namespace libanvl.opt.test;

public class OptTests
{
    [Fact]
    public void None_Are_SameInstance()
    {
        var x = Opt<object>.None;
        var y = Opt<object>.None;

        Assert.Equal(x, y);
    }

    private class Base { }
    private class Derived : Base { }
    private class NotDerived { }

    [Fact]
    public void Failed_Cast_IsNone()
    {
        var a = Opt.From(new NotDerived());
        var x = a.Cast<Base>();
        Assert.True(x.IsNone);
        Assert.False(x.IsSome);
    }

    [Fact]
    public void Cast_IsSome()
    {
        var a = Opt.From(new Derived());
        var x = a.Cast<Base>();
        Assert.True(x.IsSome);
        Assert.False(x.IsNone);
    }

    [Fact]
    public void Unwrap_ThrowsForNone()
    {
        var x = Opt<object>.None;
        Assert.Throws<OptException>(() => x.Unwrap());
    }

    [Fact]
    public void Unwrap_DoesNotThrowForSome()
    {
        var x = Opt.Some(new object());
        x.Unwrap();
    }

    [Fact]
    public void Filter_ReturnsSome_WhenPredicateIsTrue()
    {
        var opt = Opt.Some(5);
        var result = opt.Filter(x => x > 3);
        Assert.True(result.IsSome);
    }

    [Fact]
    public void Filter_ReturnsNone_WhenPredicateIsFalse()
    {
        var opt = Opt.Some(5);
        var result = opt.Filter(x => x < 3);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void AndThen_ReturnsTransformedOption_WhenSome()
    {
        var opt = Opt.Some(5);
        var result = opt.AndThen(x => Opt.Some(x * 2));
        Assert.True(result.IsSome);
        Assert.Equal(10, result.Unwrap());
    }

    [Fact]
    public void AndThen_ReturnsNone_WhenNone()
    {
        var opt = Opt<int>.None;
        var result = opt.AndThen(x => Opt.Some(x * 2));
        Assert.True(result.IsNone);
    }
}
