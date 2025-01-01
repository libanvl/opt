using Xunit;

namespace libanvl.opt.test;

public class AnyExtensionsTests
{
    [Theory]
    [MemberData(nameof(GetAggregateTestData))]
    public void Aggregate_ShouldAccumulateValues(Any<int> any, int expected)
    {
        var sum = any.Aggregate(0, (acc, x) => acc + x);
        Assert.Equal(expected, sum);
    }

    public static TheoryData<Any<int>, int> GetAggregateTestData()
    {
        return new TheoryData<Any<int>, int>
        {
            { new Any<int>(new List<int> { 1, 2, 3 }), 6 },
            { new Any<int>(42), 42 },
            { new Any<int>(), 0 }
        };
    }

    [Theory]
    [MemberData(nameof(GetAnyTestData))]
    public void Any_ShouldReturnTrueIfAnyElementMatchesPredicate(Any<int> any, bool expected)
    {
        var result = any.Any(x => x == 2);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Any<int>, bool> GetAnyTestData()
    {
        return new TheoryData<Any<int>, bool>
        {
            { new Any<int>(new List<int> { 1, 2, 3 }), true },
            { new Any<int>(42), false },
            { new Any<int>(), false }
        };
    }

    [Theory]
    [MemberData(nameof(GetSelectManyTestData))]
    public void SelectMany_ShouldProjectAndFlattenSequences(Any<int> any, Func<int, IEnumerable<int>> selector, Any<int> expected)
    {
        var result = any.SelectMany(selector);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Any<int>, Func<int, IEnumerable<int>>, Any<int>> GetSelectManyTestData()
    {
        return new TheoryData<Any<int>, Func<int, IEnumerable<int>>, Any<int>>
        {
            { new Any<int>(new List<int> { 1, 2, 3 }), x => new List<int> { x, x * 2 }, new Any<int>(new List<int> { 1, 2, 2, 4, 3, 6 }) },
            { new Any<int>(42), x => new List<int> { x, x * 2 }, new Any<int>(new List<int> { 42, 84 }) },
            { new Any<int>(), x => new List<int> { x, x * 2 }, new Any<int>() }
        };
    }

    [Theory]
    [MemberData(nameof(GetWhereTestData))]
    public void Where_ShouldFilterValuesBasedOnPredicate(Any<int> any, Func<int, bool> predicate, Any<int> expected)
    {
        var result = any.Where(predicate);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Any<int>, Func<int, bool>, Any<int>> GetWhereTestData()
    {
        return new TheoryData<Any<int>, Func<int, bool>, Any<int>>
        {
            { new Any<int>(new List<int> { 1, 2, 3 }), x => x > 1, new Any<int>(new List<int> { 2, 3 }) },
            { new Any<int>(42), x => x > 1, new Any<int>(42) },
            { new Any<int>(), x => x > 1, new Any<int>() }
        };
    }

    [Theory]
    [MemberData(nameof(GetToDictionaryTestData))]
    public void ToDictionary_ShouldCreateDictionaryFromAny(Any<int> any, Func<int, int> keySelector, Dictionary<int, int> expected)
    {
        var result = any.ToDictionary(keySelector);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Any<int>, Func<int, int>, Dictionary<int, int>> GetToDictionaryTestData()
    {
        return new TheoryData<Any<int>, Func<int, int>, Dictionary<int, int>>
        {
            { new Any<int>([1, 2, 3]), x => x, new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } } },
            { new Any<int>(42), x => x, new Dictionary<int, int> { { 42, 42 } } },
            { new Any<int>(), x => x, new Dictionary<int, int>() }
        };
    }

    [Theory]
    [MemberData(nameof(GetFirstOrDefaultTestData))]
    public void FirstOrDefault_ShouldReturnFirstMatchingElementOrDefault(Any<int> any, Func<int, bool> predicate, int expected)
    {
        var result = any.FirstOrDefault(predicate);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Any<int>, Func<int, bool>, int> GetFirstOrDefaultTestData()
    {
        return new TheoryData<Any<int>, Func<int, bool>, int>
        {
            { new Any<int>([1, 2, 3]), x => x > 1, 2 },
            { new Any<int>(42), x => x > 1, 42 },
            { new Any<int>(), x => x > 1, default }
        };
    }

    [Theory]
    [MemberData(nameof(GetAllTestData))]
    public void All_ShouldReturnTrueIfAllElementsMatchPredicate(Any<int> any, Func<int, bool> predicate, bool expected)
    {
        var result = any.All(predicate);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Any<int>, Func<int, bool>, bool> GetAllTestData()
    {
        return new TheoryData<Any<int>, Func<int, bool>, bool>
        {
            { new Any<int>(new List<int> { 1, 2, 3 }), x => x > 0, true },
            { new Any<int>(new List<int> { 1, 2, 3 }), x => x > 1, false },
            { new Any<int>(42), x => x > 0, true },
            { new Any<int>(42), x => x > 42, false },
            { new Any<int>(), x => x > 0, true }
        };
    }
}
