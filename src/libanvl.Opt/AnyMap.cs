using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace libanvl;

/// <summary>
/// Represents a dictionary-like collection that maps keys to values of type <see cref="Any{V}"/>.
/// </summary>
/// <remarks>
/// <para>
///     Instances of <see cref="AnyMap{K, V}"/> will return copies of the <see cref="Any{V}"/> values stored in the map.
///     To act on the values in the map by reference, use the <see cref="IAnyRefMap{K, V}"/> interface.
/// </para>
/// </remarks>
/// <typeparam name="K">The type of the keys in the map.</typeparam>
/// <typeparam name="V">The type of the values in the map.</typeparam>
public class AnyMap<K, V> : Dictionary<K, Any<V>>, IAnyRefMap<K, V>
    where K : notnull
    where V : notnull
{
    ref Any<V> IAnyRefMap<K, V>.GetOrAddValueRef(K key)
    {
        return ref CollectionsMarshal.GetValueRefOrAddDefault(this, key, out _);
    }

    ref Any<V> IAnyRefMap<K, V>.GetValueRef(K key)
    {
        ref Any<V> value = ref CollectionsMarshal.GetValueRefOrNullRef(this, key);
        if (Unsafe.IsNullRef(ref value))
        {
            throw new KeyNotFoundException();
        }

        return ref value;
    }

    ref Any<V> IAnyRefMap<K, V>.GetValueRef(K key, out bool found)
    {
        ref Any<V> value = ref CollectionsMarshal.GetValueRefOrNullRef(this, key);
        found = !Unsafe.IsNullRef(ref value);
        return ref value;
    }

    void IAnyRefMap<K, V>.ForEachRef(IAnyRefMap<K, V>.RefAction action)
    {
        foreach (K key in this.Keys)
        {
            ref var value = ref CollectionsMarshal.GetValueRefOrNullRef(this, key);
            Verify.Operation(!Unsafe.IsNullRef(ref value), $"Key '{key}' not found in the dictionary.");
            action(key, ref value);
        }
    }

    IAnyRefMap<K, V>.RefEnumerator IAnyRefMap<K, V>.GetEnumerator()
    {
        return new IAnyRefMap<K, V>.RefEnumerator(this);
    }
}
