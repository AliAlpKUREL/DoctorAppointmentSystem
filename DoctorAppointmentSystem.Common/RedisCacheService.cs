using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace DoctorAppointmentSystem.Common;

public class RedisCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly IConfiguration _configuration;
    public RedisCacheService(IConfiguration configuration)
    {
        _configuration = configuration;
        var redisConnectionString = _configuration.GetConnectionString("Redis")!;
        _redis = ConnectionMultiplexer.Connect(redisConnectionString);
        _database = _redis.GetDatabase();
    }

    public void SetCache(string key, string value, TimeSpan expirationTime)
    {
        _database.StringSet(key, value, expirationTime);
    }

    public string GetCache(string key)
    {
        return _database.StringGet(key);
    }

    public bool IsCacheExists(string key)
    {
        return _database.KeyExists(key);
    }
}
