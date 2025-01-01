using Xunit;

namespace libanvl.opt.test;

public class AnyMapTests
{
    [Fact]
    public void RefIndexer_ShouldGetReferenceToValue()
    {
        var map = new AnyMap<string, int> { { "key1", new Any<int>(42) } };
        ref var value = ref ((IAnyRefMap<string, int>)map)["key1"];
        Assert.Equal(42, value.Single.Unwrap());
    }

    [Fact]
    public void GetOrAddValueRef_ShouldAddValueIfKeyDoesNotExist()
    {
        var map = new AnyMap<string, int>();
        ref var value = ref ((IAnyRefMap<string, int>)map).GetOrAddValueRef("key1");
        value = new Any<int>(42);

        Assert.True(map.ContainsKey("key1"));
        Assert.Equal(42, map["key1"].Single.Unwrap());
    }

    [Fact]
    public void GetValueRef_ShouldThrowIfKeyDoesNotExist()
    {
        var map = new AnyMap<string, int>();

        Assert.Throws<KeyNotFoundException>(() => ((IAnyRefMap<string, int>)map).GetValueRef("key1"));
    }

    [Fact]
    public void GetValueRef_ShouldReturnReferenceIfKeyExists()
    {
        var map = new AnyMap<string, int> { { "key1", new Any<int>(42) } };
        ref var value = ref ((IAnyRefMap<string, int>)map).GetValueRef("key1");

        Assert.Equal(42, value.Single.Unwrap());
    }

    [Fact]
    public void ForEachRef_ShouldInvokeActionForEachElement()
    {
        var map = new AnyMap<string, int>
        {
            { "key1", new Any<int>(1) },
            { "key2", new Any<int>(2) }
        };

        var keys = new List<string>();
        var values = new List<int>();

        ((IAnyRefMap<string, int>)map).ForEachRef((string key, ref Any<int> value) =>
        {
            keys.Add(key);
            values.Add(value.Single.Unwrap());
        });

        Assert.Contains("key1", keys);
        Assert.Contains("key2", keys);
        Assert.Contains(1, values);
        Assert.Contains(2, values);
    }

    [Fact]
    public void GetEnumerator_ShouldEnumerateAllElements()
    {
        var map = new AnyMap<string, int>
        {
            { "key1", new Any<int>(1) },
            { "key2", new Any<int>(2) }
        };
        var values = new List<int>();
        foreach (var p in ((IAnyRefMap<string, int>)map))
        {
            values.Add(p.Single.Unwrap());
        }
        Assert.Contains(1, values);
        Assert.Contains(2, values);
    }
}
