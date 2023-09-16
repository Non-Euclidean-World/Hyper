using System.Collections.Concurrent;

namespace Chunks.ChunkManagement;

public class ConcurrentHashSet<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _dictionary = new();

    public bool Add(T item)
    {
        return _dictionary.TryAdd(item, 0);
    }

    public bool Remove(T item)
    {
        return _dictionary.TryRemove(item, out _);
    }

    public bool Contains(T item)
    {
        return _dictionary.ContainsKey(item);
    }
}