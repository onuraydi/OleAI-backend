

namespace OleAI.Backend.Core.CrossCuttingConcerns.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}
