using System.Collections.Concurrent;

namespace SoulWriter.Services;

public class MemoryService
{
    private readonly ConcurrentDictionary<string, string> _memory = new();

    public Task SaveAsync(string collection, string key, string text)
    {
        var fullKey = $"{collection}:{key}";
        _memory[fullKey] = text;
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(string collection, string key)
    {
        var fullKey = $"{collection}:{key}";
        _memory.TryGetValue(fullKey, out var value);
        return Task.FromResult(value);
    }

    public Task DeleteAsync(string collection, string key)
    {
        var fullKey = $"{collection}:{key}";
        _memory.TryRemove(fullKey, out _);
        return Task.CompletedTask;
    }
}
