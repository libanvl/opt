using Xunit;

namespace libanvl.opt.test;

public class AnyExtensionsTests
{
    [Fact]
    public void Select_ShouldTransformElements()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var transformed = any.Select(x => x * 2);
        Assert.True(transformed.IsMany);
        Assert.Equal(new List<int> { 2, 4, 6 }, transformed.Many.Unwrap());
    }

    [Fact]
    public void Where_ShouldFilterElements()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3, 4 });
        var filtered = any.Where(x => x % 2 == 0);
        Assert.True(filtered.IsMany);
        Assert.Equal(new List<int> { 2, 4 }, filtered.Many.Unwrap());
    }

    [Fact]
    public void SelectMany_ShouldFlattenSequences()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var flattened = any.SelectMany(x => new List<int> { x, x * 2 });
        Assert.True(flattened.IsMany);
        Assert.Equal(new List<int> { 1, 2, 2, 4, 3, 6 }, flattened.Many.Unwrap());
    }

    [Fact]
    public void Aggregate_ShouldAccumulateValues()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var sum = any.Aggregate(0, (acc, x) => acc + x);
        Assert.Equal(6, sum);
    }

    [Fact]
    public void Any_ShouldReturnTrueIfAnyElementMatchesPredicate()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var result = any.Any(x => x == 2);
        Assert.True(result);
    }

    [Fact]
    public void All_ShouldReturnTrueIfAllElementsMatchPredicate()
    {
        var any = new Any<int>(new List<int> { 2, 4, 6 });
        var result = any.All(x => x % 2 == 0);
        Assert.True(result);
    }

    [Fact]
    public void FirstOrDefault_ShouldReturnFirstMatchingElementOrDefault()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var result = any.FirstOrDefault(x => x > 1);
        Assert.Equal(2, result);
    }

    [Fact]
    public void Match_ShouldInvokeCorrectAction()
    {
        var anySingle = new Any<int>(42);
        var anyNone = new Any<int>();

        bool singleMatched = false;
        bool noneMatched = false;

        anySingle.Match(
            some => singleMatched = true,
            () => noneMatched = true
        );
        Assert.True(singleMatched);
        Assert.False(noneMatched);

        anyNone.Match(
            some => singleMatched = true,
            () => noneMatched = true
        );
        Assert.True(noneMatched);
    }

    [Fact]
    public void ToDictionary_ShouldCreateDictionaryFromElements()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var dictionary = any.ToDictionary(x => x);
        Assert.Equal(3, dictionary.Count);
        Assert.Equal(1, dictionary[1]);
        Assert.Equal(2, dictionary[2]);
        Assert.Equal(3, dictionary[3]);
    }

    [Fact]
    public void ToDictionary_ShouldReturnEmptyDictionary_WhenSourceIsNone()
    {
        var any = new Any<int>();
        var dictionary = any.ToDictionary(x => x);
        Assert.Empty(dictionary);
    }

    [Fact]
    public void ToDictionary_ShouldReturnSingleElementDictionary_WhenSourceIsSingle()
    {
        var any = new Any<int>(42);
        var dictionary = any.ToDictionary(x => x);
        Assert.Single(dictionary);
        Assert.Equal(42, dictionary[42]);
    }

    [Fact]
    public void ToDictionary_ShouldReturnDictionaryWithCorrectKeysAndValues_WhenSourceIsMany()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var dictionary = any.ToDictionary(x => x);
        Assert.Equal(3, dictionary.Count);
        Assert.Equal(1, dictionary[1]);
        Assert.Equal(2, dictionary[2]);
        Assert.Equal(3, dictionary[3]);
    }
    [Fact]
    public void Where_ShouldReturnSource_WhenSourceIsNone()
    {
        var any = new Any<int>();
        var result = any.Where(x => x > 0);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Where_ShouldReturnEmpty_WhenNoElementsMatchPredicate()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3 });
        var result = any.Where(x => x > 3);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Where_ShouldReturnSingleElement_WhenSourceIsSingleAndMatchesPredicate()
    {
        var any = new Any<int>(2);
        var result = any.Where(x => x > 1);
        Assert.True(result.IsSingle);
        Assert.Equal(2, result.Single.Unwrap());
    }

    [Fact]
    public void Where_ShouldReturnEmpty_WhenSourceIsSingleAndDoesNotMatchPredicate()
    {
        var any = new Any<int>(2);
        var result = any.Where(x => x > 2);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Where_ShouldFilterElements_WhenSourceIsMany()
    {
        var any = new Any<int>(new List<int> { 1, 2, 3, 4 });
        var result = any.Where(x => x % 2 == 0);
        Assert.True(result.IsMany);
        Assert.Equal(new List<int> { 2, 4 }, result.Many.Unwrap());
    }
}
