using Xunit;

namespace libanvl.opt.test;

public class AnyTests
{
    [Fact]
    public void Any_DefaultConstructor_ShouldBeNone()
    {
        var any = new Any<int>();
        Assert.True(any.IsNone);
        Assert.False(any.IsSome);
        Assert.False(any.IsSingle);
        Assert.False(any.IsMany);
        Assert.Equal(0, any.Count);
    }

    [Fact]
    public void Any_ConstructorWithNullEnumerable_ShouldBeNone()
    {
        var any = new Any<int>((IEnumerable<int>?)null);
        Assert.True(any.IsNone);
        Assert.False(any.IsSome);
        Assert.False(any.IsSingle);
        Assert.False(any.IsMany);
        Assert.Equal(0, any.Count);
    }

    [Fact]
    public void Any_ConstructorWithNullList_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => new Any<int>((List<int>?)null!));
    }

    [Fact]
    public void Any_ConstructorWithEmptyEnumerable_ShouldBeNone()
    {
        var any = new Any<int>(Enumerable.Empty<int>());
        Assert.True(any.IsNone);
        Assert.False(any.IsSome);
        Assert.False(any.IsSingle);
        Assert.False(any.IsMany);
        Assert.Equal(0, any.Count);
    }

    [Fact]
    public void Any_ConstructorWithSingleElementEnumerable_ShouldBeSingle()
    {
        var any = new Any<int>(new List<int> { 42 });
        Assert.False(any.IsNone);
        Assert.True(any.IsSome);
        Assert.True(any.IsSingle);
        Assert.False(any.IsMany);
        Assert.Equal(1, any.Count);
        Assert.Equal(42, any.Single.Unwrap());
    }

    [Fact]
    public void Any_ConstructorWithMultipleElementsEnumerable_ShouldBeMany()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.False(any.IsNone);
        Assert.True(any.IsSome);
        Assert.False(any.IsSingle);
        Assert.True(any.IsMany);
        Assert.Equal(3, any.Count);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_MultipleElementsConstructor_ShouldBeMany()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.False(any.IsNone);
        Assert.True(any.IsSome);
        Assert.False(any.IsSingle);
        Assert.True(any.IsMany);
        Assert.Equal(3, any.Count);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_RemoveElement_ShouldReturnTrue_WhenSingleElementIsRemoved()
    {
        var any = new Any<int>(42);
        var result = any.Remove(42);
        Assert.True(result);
        Assert.True(any.IsNone);
    }

    [Fact]
    public void Any_RemoveElement_ShouldReturnFalse_WhenSingleElementIsNotFound()
    {
        var any = new Any<int>(42);
        var result = any.Remove(43);
        Assert.False(result);
        Assert.True(any.IsSingle);
    }

    [Fact]
    public void Any_RemoveElement_ShouldReturnTrue_WhenElementIsRemovedFromMany()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var result = any.Remove(2);
        Assert.True(result);
        Assert.Equal(new List<int> { 1, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_RemoveElement_ShouldReturnFalse_WhenElementIsNotFoundInMany()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var result = any.Remove(4);
        Assert.False(result);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_AddElement_ShouldChangeState()
    {
        var any = new Any<int>();
        any.Add(42);
        Assert.True(any.IsSingle);
        Assert.Equal(42, any.Single.Unwrap());

        any.Add(43);
        Assert.True(any.IsMany);
        Assert.Equal(new List<int> { 42, 43 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_RemoveElement_ShouldChangeState()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.True(any.IsMany);

        any.Remove(2);
        Assert.Equal(new List<int> { 1, 3 }, any.Many.Unwrap());

        any.Remove(1);
        any.Remove(3);
        Assert.True(any.IsNone);
    }

    [Fact]
    public void Any_ContainsElement_ShouldReturnCorrectResult()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.True(any.Contains(2));
        Assert.False(any.Contains(4));
    }

    [Fact]
    public void Any_ToArray_ShouldReturnCorrectArray()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.Equal(new int[] { 1, 2, 3 }, any.ToArray());
    }

    [Fact]
    public void Any_ToList_ShouldReturnCorrectList()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.Equal(new List<int> { 1, 2, 3 }, any.ToList());
    }

    [Fact]
    public void Any_ToEnumerable_ShouldReturnCorrectEnumerable()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.Equal(new List<int> { 1, 2, 3 }, any.ToEnumerable());
    }

    [Fact]
    public void Any_Equality_ShouldReturnCorrectResult()
    {
        var any1 = new Any<int>(new List<int> { 1, 2, 3 });
        var any2 = new Any<int>(new List<int> { 1, 2, 3 });
        var any3 = new Any<int>(new List<int> { 4, 5, 6 });

        Assert.True(any1 == any2);
        Assert.False(any1 == any3);
        Assert.True(any1 != any3);
    }

    [Fact]
    public void Any_FromSingle_ShouldCreateSingle()
    {
        var any = Any.From(42);
        Assert.True(any.IsSingle);
        Assert.Equal(42, any.Single.Unwrap());
    }

    [Fact]
    public void Any_FromList_ShouldCreateMany()
    {
        var any = Any.From(new List<int> { 1, 2, 3 });
        Assert.True(any.IsMany);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_FromEnumerable_ShouldCreateMany()
    {
        var any = Any.From((IEnumerable<int>)new List<int> { 1, 2, 3 });
        Assert.True(any.IsMany);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_FromParams_ShouldCreateMany()
    {
        var any = Any.From(1, 2, 3);
        Assert.True(any.IsMany);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_CreateFromSpan_ShouldCreateMany()
    {
        var span = new ReadOnlySpan<int>(new int[] { 1, 2, 3 });
        Any<int> any = new(span);
        Assert.True(any.IsMany);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_Cast_ShouldCastElements()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var casted = any.Cast<object>();
        Assert.True(casted.IsMany);
        Assert.Equal(new List<object> { 1, 2, 3 }, casted.Many.Unwrap());
    }

    [Fact]
    public void Any_ImplicitOperatorSingle_ShouldCreateSingle()
    {
        Any<int> any = 42;
        Assert.True(any.IsSingle);
        Assert.Equal(42, any.Single.Unwrap());
    }

    [Fact]
    public void Any_ImplicitOperatorList_ShouldCreateMany()
    {
        Any<int> any = new List<int> { 1, 2, 3 };
        Assert.True(any.IsMany);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_ImplicitOperatorToSingle_ShouldReturnSingle()
    {
        var any = new Any<int>(42);
        int value = any;
        Assert.Equal(42, value);
    }

    [Fact]
    public void Any_ImplicitOperatorToList_ShouldReturnList()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        List<int> list = any;
        Assert.Equal(new List<int> { 1, 2, 3 }, list);
    }

    [Fact]
    public void Any_Match_ShouldInvokeCorrectAction()
    {
        var anySingle = new Any<int>(42);
        var anyMany = new Any<int>(new List<int> { 1, 2, 3 });
        var anyNone = new Any<int>();

        bool singleMatched = false;
        bool manyMatched = false;
        bool noneMatched = false;

        anySingle.Match(
            some => singleMatched = true,
            () => noneMatched = true
        );
        Assert.True(singleMatched);
        Assert.False(noneMatched);

        anyMany.Match(
            some => manyMatched = true,
            () => noneMatched = true
        );
        Assert.True(manyMatched);
        Assert.False(noneMatched);

        anyNone.Match(
            some => singleMatched = true,
            () => noneMatched = true
        );
        Assert.True(noneMatched);
    }

    [Fact]
    public void Any_Select_ShouldTransformElements()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var transformed = any.Select(x => x * 2);
        Assert.True(transformed.IsMany);
        Assert.Equal(new List<int> { 2, 4, 6 }, transformed.Many.Unwrap());
    }
    [Fact]
    public void Any_AddElement_ShouldAddToNone()
    {
        var any = new Any<int>();
        any.Add(42);
        Assert.True(any.IsSingle);
        Assert.Equal(42, any.Single.Unwrap());
    }

    [Fact]
    public void Any_AddElement_ShouldAddToSingle()
    {
        var any = new Any<int>(42);
        any.Add(43);
        Assert.True(any.IsMany);
        Assert.Equal(new List<int> { 42, 43 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_AddElement_ShouldAddToMany()
    {
        var any = new Any<int>(new List<int> { 1, 2 });
        any.Add(3);
        Assert.True(any.IsMany);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }

    [Fact]
    public void Any_CanBeEnumerated()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var sum = 0;
        foreach (var item in any)
        {
            sum += item;
        }
        Assert.Equal(6, sum);
    }
    [Fact]
    public void Any_ToOpt_ShouldReturnNone_WhenEmpty()
    {
        var any = new Any<int>();
        var opt = any.ToOpt();
        Assert.True(opt.IsNone);
    }

    [Fact]
    public void Any_ToOpt_ShouldReturnSingle_WhenSingleElement()
    {
        var any = new Any<int>(42);
        var opt = any.ToOpt();
        Assert.True(opt.IsSome);
        Assert.Equal(42, opt.Unwrap().Single());
    }

    [Fact]
    public void Any_ToOpt_ShouldReturnMany_WhenMultipleElements()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var opt = any.ToOpt();
        Assert.True(opt.IsSome);
        Assert.Equal(new List<int> { 1, 2, 3 }, opt.Unwrap());
    }
}
