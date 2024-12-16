using System.Collections.Generic;
using System.Linq;
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
}