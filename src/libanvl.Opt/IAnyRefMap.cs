using System.Runtime.InteropServices;

namespace libanvl;

/// <summary>
/// Represents a map that provides references to values associated with keys.
/// </summary>
/// <typeparam name="K">The type of the keys in the map.</typeparam>
/// <typeparam name="V">The type of the values in the map.</typeparam>
public interface IAnyRefMap<K, V>
    where K : notnull
    where V : notnull
{
    /// <summary>
    /// Represents a delegate that performs an action on a key and a reference to a value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The reference to the value.</param>
    public delegate void RefAction(K key, ref Any<V> value);

    /// <summary>
    /// Gets a reference to the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <returns>A reference to the value associated with the specified key.</returns>
    ref Any<V> this[K key] => ref GetValueRef(key);

    /// <summary>
    /// Gets a reference to the value associated with the specified key, or adds a new value if the key does not exist.
    /// </summary>
    /// <param name="key">The key whose value to get or add.</param>
    /// <returns>A reference to the value associated with the specified key.</returns>
    ref Any<V> GetOrAddValueRef(K key);

    /// <summary>
    /// Gets a reference to the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <returns>A reference to the value associated with the specified key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the key does not exist in the map.</exception>
    ref Any<V> GetValueRef(K key);

    /// <summary>
    /// Tries to get a reference to the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">When this method returns, contains the reference to the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the map contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
    bool TryGetValueRef(K key, ref Any<V> value);

    /// <summary>
    /// Performs the specified action on each key and reference to a value in the map.
    /// </summary>
    /// <param name="action">The action to perform on each key and reference to a value.</param>
    void ForEachRef(RefAction action);

    /// <summary>
    /// Returns an enumerator that iterates through the map, providing references to the values.
    /// </summary>
    /// <returns>An enumerator for the map.</returns>
    RefEnumerator GetEnumerator();

    /// <summary>
    /// Enumerates the elements of an <see cref="IAnyRefMap{K, V}"/>, providing references to the values.
    /// </summary>
    /// <param name="map">The map to enumerate.</param>
    public struct RefEnumerator(AnyMap<K, V> map)
    {
        private Dictionary<K, Any<V>>.Enumerator _enumerator = map.GetEnumerator();

        /// <summary>
        /// Gets a reference to the current value in the enumerator.
        /// </summary>
        public ref Any<V> Current
        {
            get
            {
                return ref CollectionsMarshal.GetValueRefOrNullRef(map, _enumerator.Current.Key);
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the map.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the map.</returns>
        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the map.
        /// </summary>
        public void Reset()
        {
            _enumerator = map.GetEnumerator();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="RefEnumerator"/>.
        /// </summary>
        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}
