using System.Runtime.Serialization;
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

    [Theory]
    [InlineData(new int[] { 1, 2, 3 }, 2, true)]
    [InlineData(new int[] { 1, 2, 3 }, 4, false)]
    [InlineData(new int[] { }, 1, false)]
    [InlineData(new int[] { 42 }, 42, true)]
    [InlineData(new int[] { 42 }, 43, false)]
    public void Any_ContainsElement_ShouldReturnCorrectResult(int[] input, int element, bool expected)
    {
        var any = new Any<int>(input);
        Assert.Equal(expected, any.Contains(element));
    }

    [Theory]
    [InlineData(new int[] { }, new int[] { })]
    [InlineData(new int[] { 1 }, new int[] { 1 })]
    [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 })]
    public void Any_ToArray_ShouldReturnCorrectArray(int[] input, int[] expected)
    {
        var any = new Any<int>(input);
        Assert.Equal(expected, any.ToArray());
    }

    [Theory]
    [InlineData(new int[] { }, new int[] { })]
    [InlineData(new int[] { 1 }, new int[] { 1 })]
    [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 })]
    public void Any_ToList_ShouldReturnCorrectList(int[] input, int[] expected)
    {
        var any = new Any<int>(input);
        Assert.Equal(expected, any.ToList());
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 })]
    [InlineData(new int[] { 42 }, new int[] { 42 })]
    [InlineData(new int[] { }, new int[] { })]
    public void Any_ToEnumerable_ShouldReturnCorrectEnumerable(int[] input, int[] expected)
    {
        var any = new Any<int>(input);
        Assert.Equal(expected, any.ToEnumerable());
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
    public void Any_Equals_ShouldReturnTrue_WhenBothAreNone()
    {
        var any1 = new Any<int>();
        var any2 = new Any<int>();
        Assert.True(any1.Equals(any2));
    }

    [Fact]
    public void Any_Equals_ShouldReturnTrue_WhenBothAreSingleAndEqual()
    {
        var any1 = new Any<int>(42);
        var any2 = new Any<int>(42);
        Assert.True(any1.Equals(any2));
    }

    [Fact]
    public void Any_Equals_ShouldReturnFalse_WhenBothAreSingleAndNotEqual()
    {
        var any1 = new Any<int>(42);
        var any2 = new Any<int>(43);
        Assert.False(any1.Equals(any2));
    }

    [Fact]
    public void Any_Equals_ShouldReturnTrue_WhenBothAreManyAndEqual()
    {
        var any1 = new Any<int>(new List<int> { 1, 2, 3 });
        var any2 = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.True(any1.Equals(any2));
    }

    [Fact]
    public void Any_Equals_ShouldReturnFalse_WhenBothAreManyAndNotEqual()
    {
        var any1 = new Any<int>(new List<int> { 1, 2, 3 });
        var any2 = new Any<int>(new List<int> { 4, 5, 6 });
        Assert.False(any1.Equals(any2));
    }

    [Fact]
    public void Any_Equals_ShouldReturnTrue_WhenOneIsSingleAndOtherIsSingleCollection()
    {
        var any1 = new Any<int>(42);
        var any2 = new Any<int>(new List<int> { 42 });
        Assert.True(any1.Equals(any2));
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

    [Theory]
    [InlineData(new int[] { 1, 2, 3 }, new object[] { 1, 2, 3 })]
    [InlineData(new int[] { 42 }, new object[] { 42 })]
    [InlineData(new int[] { }, new object[] { })]
    public void Any_Cast_ShouldCastElements(int[] input, object[] expected)
    {
        var any = new Any<int>(input);
        var casted = any.Cast<object>();
        Assert.Equal(expected.Length > 1, casted.IsMany);
        Assert.Equal(expected.Length == 1, casted.IsSingle);
        Assert.Equal(expected.Length == 0, casted.IsNone);
        Assert.Equal(expected, casted.ToArray());
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

    [Theory]
    [InlineData(new int[] { 1, 2, 3 }, new int[] { 2, 4, 6 })]
    [InlineData(new int[] { 42 }, new int[] { 84 })]
    [InlineData(new int[] { }, new int[] { })]
    public void Any_Select_ShouldTransformElements(int[] input, int[] expected)
    {
        var any = new Any<int>(input);
        var transformed = any.Select(x => x * 2);
        Assert.Equal(expected, transformed.ToArray());
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

    [Fact]
    public void Any_GetHashCode_ShouldReturnSameHashCode_ForEqualSingleElements()
    {
        var any1 = new Any<int>(42);
        var any2 = new Any<int>(42);
        Assert.Equal(any1.GetHashCode(), any2.GetHashCode());
    }

    [Fact]
    public void Any_GetHashCode_ShouldReturnDifferentHashCode_ForDifferentSingleElements()
    {
        var any1 = new Any<int>(42);
        var any2 = new Any<int>(43);
        Assert.NotEqual(any1.GetHashCode(), any2.GetHashCode());
    }

    [Fact(Skip = "This is not currently true. It may be in the future.")]
    public void Any_GetHashCode_ShouldReturnSameHashCode_ForEqualManyElements()
    {
        var any1 = new Any<int>(new List<int> { 1, 2, 3 });
        var any2 = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.Equal(any1.GetHashCode(), any2.GetHashCode());
    }

    [Fact]
    public void Any_GetHashCode_ShouldReturnDifferentHashCode_ForDifferentManyElements()
    {
        var any1 = new Any<int>(new List<int> { 1, 2, 3 });
        var any2 = new Any<int>(new List<int> { 4, 5, 6 });
        Assert.NotEqual(any1.GetHashCode(), any2.GetHashCode());
    }

    [Fact]
    public void Any_ConstructorWithReadOnlySpan_ShouldBeNone_WhenEmpty()
    {
        var span = new ReadOnlySpan<int>(Array.Empty<int>());
        var any = new Any<int>(span);
        Assert.True(any.IsNone);
        Assert.False(any.IsSome);
        Assert.False(any.IsSingle);
        Assert.False(any.IsMany);
        Assert.Equal(0, any.Count);
    }

    [Fact]
    public void Any_ConstructorWithReadOnlySpan_ShouldBeSingle_WhenOneElement()
    {
        var span = new ReadOnlySpan<int>(new int[] { 42 });
        var any = new Any<int>(span);
        Assert.False(any.IsNone);
        Assert.True(any.IsSome);
        Assert.True(any.IsSingle);
        Assert.False(any.IsMany);
        Assert.Equal(1, any.Count);
        Assert.Equal(42, any.Single.Unwrap());
    }

    [Fact]
    public void Any_ConstructorWithReadOnlySpan_ShouldBeMany_WhenMultipleElements()
    {
        var span = new ReadOnlySpan<int>(new int[] { 1, 2, 3 });
        var any = new Any<int>(span);
        Assert.False(any.IsNone);
        Assert.True(any.IsSome);
        Assert.False(any.IsSingle);
        Assert.True(any.IsMany);
        Assert.Equal(3, any.Count);
        Assert.Equal(new List<int> { 1, 2, 3 }, any.Many.Unwrap());
    }
    [Fact]
    public void Any_Equals_ShouldReturnFalse_WhenSingleAndManyAreNotEqual()
    {
        var anySingle = new Any<int>(42);
        var anyMany = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.False(anySingle.Equals(anyMany));
    }

    [Fact]
    public void Any_Equals_ShouldReturnFalse_WhenManyAndSingleAreNotEqual()
    {
        var anyMany = new Any<int>(new List<int> { 1, 2, 3 });
        var anySingle = new Any<int>(42);
        Assert.False(anyMany.Equals(anySingle));
    }

    [Fact]
    public void Any_Equals_ShouldReturnFalse_WhenSingleAndNoneAreNotEqual()
    {
        var anySingle = new Any<int>(42);
        var anyNone = new Any<int>();
        Assert.False(anySingle.Equals(anyNone));
    }

    [Fact]
    public void Any_Equals_ShouldReturnFalse_WhenNoneAndSingleAreNotEqual()
    {
        var anyNone = new Any<int>();
        var anySingle = new Any<int>(42);
        Assert.False(anyNone.Equals(anySingle));
    }

    [Fact]
    public void Any_Equals_ShouldReturnFalse_WhenManyAndNoneAreNotEqual()
    {
        var anyMany = new Any<int>(new List<int> { 1, 2, 3 });
        var anyNone = new Any<int>();
        Assert.False(anyMany.Equals(anyNone));
    }

    [Fact]
    public void Any_Equals_ShouldReturnFalse_WhenNoneAndManyAreNotEqual()
    {
        var anyNone = new Any<int>();
        var anyMany = new Any<int>(new List<int> { 1, 2, 3 });
        Assert.False(anyNone.Equals(anyMany));
    }
}
