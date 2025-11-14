using System.Runtime.CompilerServices;

#if LANGVER_13
namespace LangVer_13;
#elif LANGVER_14
namespace LangVer_14;
#endif

internal readonly record struct KeyId(string Value)
{
    public static implicit operator string(KeyId id) => id.Value;
}

internal class Cache_WithPriorityObject
{
    [OverloadResolutionPriority(1)]
    public bool TryGetValue(object key, out object? result)
    {
        result = key;
        return true;
    }

    public bool TryGetValue(System.ReadOnlySpan<char> key, out object? value)
    {
        value = key.ToString();
        return true;
    }

}

internal class Cache_WithPriorityROS
{
    public bool TryGetValue(object key, out object? result)
    {
        result = key;
        return true;
    }

    [OverloadResolutionPriority(1)]
    public bool TryGetValue(System.ReadOnlySpan<char> key, out object? value)
    {
        value = key.ToString();
        return true;
    }

}

[TestClass]
public sealed class OverloadResolutionPriorityTests
{
    [TestMethod]
    public void PriorityObject_WithoutCast()
    {
        Cache_WithPriorityObject cache = new();
        KeyId myId = new("test_id");

        // Resolves to MemoryCache.TryGetValue(ReadOnlySpan<char> key, out object value)
        // This results in trying to look up the cache entry with a string key "test_id"
        // rather than the KeyId struct key, leading to a cache miss.
        cache.TryGetValue(myId, out var value);
        Assert.AreEqual(myId, value);
    }

    [TestMethod]
    public void PriorityObject_WithCast()
    {
        Cache_WithPriorityObject cache = new();
        KeyId myId = new("test_id");

        // This coerces resolution to MemoryCache.TryGetValue(object key, out object value)
        // Lookup is done with the KeyIdWithImplicit struct key, leading to a cache hit.
        cache.TryGetValue((object)myId, out var value);
        Assert.AreEqual(myId, value);
    }

    [TestMethod]
    public void PriorityROS_WithoutCast()
    {
        Cache_WithPriorityROS cache = new();
        KeyId myId = new("test_id");

        // Resolves to MemoryCache.TryGetValue(ReadOnlySpan<char> key, out object value)
        // This results in trying to look up the cache entry with a string key "test_id"
        // rather than the KeyId struct key, leading to a cache miss.
        cache.TryGetValue(myId, out var value);
        Assert.AreEqual(myId, value);
    }

    [TestMethod]
    public void PriorityROS_WithCast()
    {
        Cache_WithPriorityROS cache = new();
        KeyId myId = new("test_id");

        // This coerces resolution to MemoryCache.TryGetValue(object key, out object value)
        // Lookup is done with the KeyIdWithImplicit struct key, leading to a cache hit.
        cache.TryGetValue((object)myId, out var value);
        Assert.AreEqual(myId, value);
    }
}