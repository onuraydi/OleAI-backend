using System.Text.Json;
using StackExchange.Redis;

namespace OleAI.Backend.Core.CrossCuttingConcerns.Caching.Redis
{
    public class RedisCacheManager : ICacheService
    {
        private readonly IDatabase _database;

        // ConnectionMultiplexer'ı DI (Dependency Injection) container üzerinden alıyoruz.
        // Bu sayede Redis bağlantısı uygulama ayağa kalktığında bir kez (Singleton) kurulur.
        public RedisCacheManager(IConnectionMultiplexer redisConnection)
        {
            _database = redisConnection.GetDatabase();
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);

            if (!value.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);

            // SENIOR CHECK: 3. parametrenin adını "expiry:" olarak açıkça belirtiyoruz.
            // StackExchange.Redis güncel sürümlerinde TimeSpan? overload'unu kullanabilmek için 
            // keepTtl parametresini de belirtmemiz gerekiyor.
            await _database.StringSetAsync(
                key: key,
                value: serializedValue,
                expiry: absoluteExpiration ?? slidingExpiration,
                keepTtl: false
            );
        }
    }
}
