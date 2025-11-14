using Microsoft.Extensions.Caching.Memory;
using FluentAssertions;

#if MEMORYCACHE_9 && LANGVER_13
namespace MemoryCache_v9_LangVer_13;
#elif MEMORYCACHE_9 && LANGVER_14
namespace MemoryCache_v9_LangVer_14;
#elif MEMORYCACHE_10 && LANGVER_13
namespace MemoryCache_v10_LangVer_13;
#elif MEMORYCACHE_10 && LANGVER_14
namespace MemoryCache_v10_LangVer_14;
#endif

internal readonly record struct KeyId(string Value)
{
    public static implicit operator string(KeyId id) => id.Value;
}

[TestClass]
public sealed class MemoryCacheTests
{
    [TestMethod]
    public void Fails_on_CSharp14_with_MemoryCache_v10()
    {
        KeyId myId = new("test_id");
        using MemoryCache cache = new(new MemoryCacheOptions());
        cache.Set(myId, "value");

        // Resolves to MemoryCache.TryGetValue(ReadOnlySpan<char> key, out object value)
        // This results in trying to look up the cache entry with a string key "test_id"
        // rather than the KeyId struct key, leading to a cache miss.
        cache.TryGetValue(myId, out var value).Should().BeTrue();
    }

    [TestMethod]
    public void PassesEverywhere()
    {
        KeyId myId = new("test_id");
        using MemoryCache cache = new(new MemoryCacheOptions());
        cache.Set(myId, "value");

        // This coerces resolution to MemoryCache.TryGetValue(object key, out object value)
        // Lookup is done with the KeyIdWithImplicit struct key, leading to a cache hit.
        cache.TryGetValue((object)myId, out var value).Should().BeTrue();
    }
}