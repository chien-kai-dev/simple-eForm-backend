using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

public interface IRedisCacheService
{
    Task RemoveCacheValueAsync(string key);
    Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration);
    Task<T> GetCacheValueAsync<T>(string key);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task RemoveCacheValueAsync(string key)
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
    
    public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
    {
        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, expiration);
    }

    public async Task<T> GetCacheValueAsync<T>(string key)
    {
        var db = _redis.GetDatabase();
        var json = await db.StringGetAsync(key);
        Console.WriteLine($"Result: {json}");
        return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
    }
}
