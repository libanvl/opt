using Xunit;

namespace libanvl.opt.test;

public class OneOrManyTests
{
    [Fact]
    public void SingleElement_Constructor_ShouldSetSingle()
    {
        var single = new OneOrMany<int>(5);
        Assert.True(single.IsSingle);
        Assert.False(single.IsMany);
        Assert.Equal(1, single.Count);
        Assert.Equal(5, single.Single);
    }

    [Fact]
    public void MultipleElements_Constructor_ShouldSetMany()
    {
        var many = new OneOrMany<int>(new List<int> { 1, 2, 3 });
        Assert.False(many.IsSingle);
        Assert.True(many.IsMany);
        Assert.Equal(3, many.Count);
        Assert.Equal(new List<int> { 1, 2, 3 }, many.Many);
    }

    [Fact]
    public void Add_SingleElement_ShouldConvertToMany()
    {
        var oneOrMany = new OneOrMany<int>(5);
        oneOrMany.Add(10);
        Assert.False(oneOrMany.IsSingle);
        Assert.True(oneOrMany.IsMany);
        Assert.Equal(2, oneOrMany.Count);
        Assert.Equal(new List<int> { 5, 10 }, oneOrMany.Many);
    }


    [Fact]
    public void Remove_MultipleElements_ShouldUpdateContainer()
    {
        var oneOrMany = new OneOrMany<int>(new List<int> { 1, 2, 3 });
        var removed = oneOrMany.Remove(2);
        Assert.True(removed);
        Assert.True(oneOrMany.IsMany);
        Assert.Equal(2, oneOrMany.Count);
        Assert.Equal(new List<int> { 1, 3 }, oneOrMany.Many);
    }

    [Fact]
    public void Contains_ShouldReturnCorrectResult()
    {
        var oneOrMany = new OneOrMany<int>(new List<int> { 1, 2, 3 });
        Assert.True(oneOrMany.Contains(2));
        Assert.False(oneOrMany.Contains(4));
    }

    [Fact]
    public void ToArray_ShouldReturnCorrectArray()
    {
        var oneOrMany = new OneOrMany<int>(new List<int> { 1, 2, 3 });
        var array = oneOrMany.ToArray();
        Assert.Equal(new int[] { 1, 2, 3 }, array);
    }

    [Fact]
    public void ToList_ShouldReturnCorrectList()
    {
        var oneOrMany = new OneOrMany<int>(new List<int> { 1, 2, 3 });
        var list = oneOrMany.ToList();
        Assert.Equal(new List<int> { 1, 2, 3 }, list);
    }

    [Fact]
    public void Enumerator_ShouldEnumerateCorrectly()
    {
        var oneOrMany = new OneOrMany<int>(new List<int> { 1, 2, 3 });
        var enumerator = oneOrMany.GetEnumerator();
        var elements = new List<int>();
        while (enumerator.MoveNext())
        {
            elements.Add(enumerator.Current);
        }
        Assert.Equal(new List<int> { 1, 2, 3 }, elements);
    }

    [Fact]
    public void Equality_ShouldReturnCorrectResult()
    {
        var oneOrMany1 = new OneOrMany<int>(new List<int> { 1, 2, 3 });
        var oneOrMany2 = new OneOrMany<int>(new List<int> { 1, 2, 3 });
        var oneOrMany3 = new OneOrMany<int>(new List<int> { 4, 5, 6 });
        Assert.True(oneOrMany1 == oneOrMany2);
        Assert.False(oneOrMany1 == oneOrMany3);
    }
}
