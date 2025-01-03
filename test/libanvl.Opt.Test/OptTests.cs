using libanvl.Exceptions;
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
    [Fact]
    public void CompareTo_ReturnsNegative_WhenThisIsSomeAndOtherIsNone()
    {
        var opt1 = Opt.Some(5);
        var opt2 = Opt<int>.None;
        Assert.True(opt1.CompareTo(opt2) < 0);
    }

    [Fact]
    public void CompareTo_ReturnsPositive_WhenThisIsNoneAndOtherIsSome()
    {
        var opt1 = Opt<int>.None;
        var opt2 = Opt.Some(5);
        Assert.True(opt1.CompareTo(opt2) > 0);
    }

    [Fact]
    public void CompareTo_ReturnsZero_WhenBothAreNone()
    {
        var opt1 = Opt<int>.None;
        var opt2 = Opt<int>.None;
        Assert.Equal(0, opt1.CompareTo(opt2));
    }

    [Fact]
    public void CompareTo_ReturnsComparisonResult_WhenBothAreSome()
    {
        var opt1 = Opt.Some(5);
        var opt2 = Opt.Some(10);
        Assert.True(opt1.CompareTo(opt2) < 0);
    }
    [Fact]
    public void ToString_ReturnsSome_WhenIsSome()
    {
        var opt = Opt.Some(5);
        Assert.Equal("Some(5)", opt.ToString());
    }

    [Fact]
    public void ToString_ReturnsNone_WhenIsNone()
    {
        var opt = Opt<int>.None;
        Assert.Equal("None", opt.ToString());
    }

    [Fact]
    public void Match_InvokesSomeFunc_WhenIsSome()
    {
        var opt = Opt.Some(5);
        bool someInvoked = false;
        bool noneInvoked = false;

        opt.Match(
            some: _ => someInvoked = true,
            none: () => noneInvoked = true
        );

        Assert.True(someInvoked);
        Assert.False(noneInvoked);
    }

    [Fact]
    public void Match_InvokesNoneFunc_WhenIsNone()
    {
        var opt = Opt<int>.None;
        bool someInvoked = false;
        bool noneInvoked = false;

        opt.Match(
            some: _ => someInvoked = true,
            none: () => noneInvoked = true
        );

        Assert.False(someInvoked);
        Assert.True(noneInvoked);
    }

    [Fact]
    public void Match_InvokesSomeAction_WhenIsSome()
    {
        var opt = Opt.Some(5);
        bool someInvoked = false;
        bool noneInvoked = false;

        opt.Match(
            some: _ => { someInvoked = true; },
            none: () => { noneInvoked = true; }
        );

        Assert.True(someInvoked);
        Assert.False(noneInvoked);
    }

    [Fact]
    public void Match_InvokesNoneAction_WhenIsNone()
    {
        var opt = Opt<int>.None;
        bool someInvoked = false;
        bool noneInvoked = false;

        opt.Match(
            some: _ => { someInvoked = true; },
            none: () => { noneInvoked = true; }
        );

        Assert.False(someInvoked);
        Assert.True(noneInvoked);
    }
    [Fact]
    public void Equals_ReturnsTrue_WhenBothAreNone()
    {
        var opt1 = Opt<int>.None;
        var opt2 = Opt<int>.None;
        Assert.True(opt1.Equals(opt2));
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenOneIsSomeAndOtherIsNone()
    {
        var opt1 = Opt.Some(5);
        var opt2 = Opt<int>.None;
        Assert.False(opt1.Equals(opt2));
    }

    [Fact]
    public void Equals_ReturnsTrue_WhenBothAreSomeWithSameValue()
    {
        var opt1 = Opt.Some(5);
        var opt2 = Opt.Some(5);
        Assert.True(opt1.Equals(opt2));
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenBothAreSomeWithDifferentValues()
    {
        var opt1 = Opt.Some(5);
        var opt2 = Opt.Some(10);
        Assert.False(opt1.Equals(opt2));
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_WhenBothAreNone()
    {
        var opt1 = Opt<int>.None;
        var opt2 = Opt<int>.None;
        Assert.Equal(opt1.GetHashCode(), opt2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_WhenBothAreSomeWithSameValue()
    {
        var opt1 = Opt.Some(5);
        var opt2 = Opt.Some(5);
        Assert.Equal(opt1.GetHashCode(), opt2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCode_WhenBothAreSomeWithDifferentValues()
    {
        var opt1 = Opt.Some(5);
        var opt2 = Opt.Some(10);
        Assert.NotEqual(opt1.GetHashCode(), opt2.GetHashCode());
    }
    [Fact]
    public void GetHashCode_ReturnsSameHashCode_ForSameValues()
    {
        var opt1 = Opt.Some(5);
        var opt2 = Opt.Some(5);
        Assert.Equal(opt1.GetHashCode(), opt2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCode_ForDifferentValues()
    {
        var opt1 = Opt.Some(5);
        var opt2 = Opt.Some(10);
        Assert.NotEqual(opt1.GetHashCode(), opt2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_ForNone()
    {
        var opt1 = Opt<int>.None;
        var opt2 = Opt<int>.None;
        Assert.Equal(opt1.GetHashCode(), opt2.GetHashCode());
    }

    [Fact]
    public void Select_ReturnsTransformedOption_WhenSome()
    {
        var opt = Opt.Some(5);
        var result = opt.Select(x => x * 2);
        Assert.True(result.IsSome);
        Assert.Equal(10, result.Unwrap());
    }

    [Fact]
    public void Select_ReturnsNone_WhenTransformedValueIsNull()
    {
        var opt = Opt.Some("Hello");
        var result = opt.Select<string>(x => null);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Select_ReturnsNone_WhenTransformedValueIsNullValueType()
    {
        Opt<int> opt = 5;
        var result = opt.Select<int>(_ => null);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Select_ReturnsNone_WhenNone()
    {
        var opt = Opt<int>.None;
        var result = opt.Select(x => x * 2);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Where_ReturnsSome_WhenPredicateIsTrue()
    {
        var opt = Opt.Some(5);
        var result = opt.Where(x => x > 3);
        Assert.True(result.IsSome);
    }

    [Fact]
    public void Where_ReturnsNone_WhenPredicateIsFalse()
    {
        var opt = Opt.Some(5);
        var result = opt.Where(x => x < 3);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Where_ReturnsNone_WhenOptionIsNone()
    {
        var opt = Opt<int>.None;
        var result = opt.Where(x => x > 3);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Cast_ReturnsSome_WhenCastIsValid()
    {
        var opt = Opt.Some((object)5);
        var result = opt.Cast<int>();
        Assert.True(result.IsSome);
        Assert.Equal(5, result.Unwrap());
    }

    [Fact]
    public void Cast_ReturnsNone_WhenCastIsInvalid()
    {
        var opt = Opt.Some((object)"string");
        var result = opt.Cast<int>();
        Assert.True(result.IsNone);
    }
}
