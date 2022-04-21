using Microsoft.Extensions.Caching.Distributed;

namespace PlatformService.Data.Cache
{
    public interface ICacheRepository<T>
    {
        Task SetAsync(string key, T value, DistributedCacheEntryOptions options);
        Task SetAsync(string key, IEnumerable<T> value, DistributedCacheEntryOptions options);
        Task RemoveAsync(string key);

        Task UpdateAsync(string key, T value, DistributedCacheEntryOptions options);

        Task<T?> GetAsync(string key);
        Task<IEnumerable<T>?> GetListAsync(string key);
    }
}