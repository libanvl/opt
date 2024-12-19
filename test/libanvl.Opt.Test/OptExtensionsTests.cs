using Xunit;

namespace libanvl.opt.test;

public class OptExtensionsTests
{

    [Fact]
    public void SomeOrEmptyTest_string()
    {
        var x = Opt<string>.None;
        Assert.Equal(string.Empty, x.SomeOr(string.Empty));
    }

    [Fact]
    public void SomeOrEmptyTest_enumerable()
    {
        var x = Opt<IEnumerable<object>>.None;
        Assert.Empty(x.SomeOr(Enumerable.Empty<object>));
    }

    [Fact]
    public void OptObjectNone_SomeOrNull_IsNull()
    {
        var x = Opt<object>.None;
        Assert.Null(x.SomeOrDefault());
    }
    [Fact]
    public void AsOpts_NullableReferenceTypes()
    {
        var source = new string?[] { "a", null, "b" };
        var result = source.AsOpts().ToList();

        Assert.Equal(3, result.Count);
        Assert.True(result[0].IsSome);
        Assert.False(result[1].IsSome);
        Assert.True(result[2].IsSome);
        Assert.Equal("a", result[0].Unwrap());
        Assert.Equal("b", result[2].Unwrap());
    }

    [Fact]
    public void AsOpts_NullableValueTypes()
    {
        var source = new int?[] { 1, null, 2 };
        var result = source.AsOpts().ToList();

        Assert.Equal(3, result.Count);
        Assert.True(result[0].IsSome);
        Assert.False(result[1].IsSome);
        Assert.True(result[2].IsSome);
        Assert.Equal(1, result[0].Unwrap());
        Assert.Equal(2, result[2].Unwrap());
    }

    [Fact]
    public void AsEnumerable_OptInstances()
    {
        var source = new List<Opt<int>> { Opt.Some(1), Opt<int>.None, Opt.Some(2) };
        var result = source.AsEnumerable().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0]);
        Assert.Equal(2, result[1]);
    }
}