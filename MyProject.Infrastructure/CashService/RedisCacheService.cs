using System.Text.Json;
using StackExchange.Redis;

namespace MyProject.Infrastructure.CashService;

public class RedisCacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        if (expiry.HasValue)
        {
            await _db.StringSetAsync(key, json, (StackExchange.Redis.Expiration)expiry.Value);
            return;
        }

        await _db.StringSetAsync(key, json);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}
