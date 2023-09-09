using System.Collections.Concurrent;

namespace Chunks;

public class ConcurrentHashSet<T> where T : notnull
{
    private ConcurrentDictionary<T, byte> _dictionary = new();
    
    public bool TryAdd(T item)
    {
        return _dictionary.TryAdd(item, 0);
    }

    public bool TryRemove(T item)
    {
        return _dictionary.TryRemove(item, out _);
    }
    
    public bool Contains(T item)
    {
        return _dictionary.ContainsKey(item);
    }
}