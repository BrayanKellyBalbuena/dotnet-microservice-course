using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace PlatformService.Data.Cache
{
    public class CacheRepository<T> : ICacheRepository<T>
    {
        private readonly IDistributedCache cache;

        public CacheRepository(IDistributedCache cache)
        {
            this.cache = cache;
        }
 
        public virtual Task<T?> GetAsync(string key)
        {
            var value =  cache.GetStringAsync(key).Result;
 
            if (value != null)
            {
                return Task.FromResult<T?>(JsonSerializer.Deserialize<T>(value));
            }
 
            return Task.FromResult<T?>(default);
        }

        public virtual Task<IEnumerable<T>?> GetListAsync(string key)
        {
            var value =  cache.GetStringAsync(key).Result;
 
            if (value != null)
            {
                return Task.FromResult<IEnumerable<T>?>(JsonSerializer.Deserialize<IEnumerable<T>?>(value));
            }
 
            return Task.FromResult<IEnumerable<T>?>(default);
        }

       public virtual Task SetAsync(string key, T value, DistributedCacheEntryOptions options)
       {  
            return cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
       }

       public Task SetAsync(string key, IEnumerable<T> value, DistributedCacheEntryOptions options)
        {
             return cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }

       public virtual Task RemoveAsync(string key)
       { 
          return cache.RemoveAsync(key);
       }

        public Task UpdateAsync(string key, T value, DistributedCacheEntryOptions options)
        {
            var isRemoveSuccessful = RemoveAsync(key).IsCompletedSuccessfully;

            if(isRemoveSuccessful)
            {
                return SetAsync(key, value, options);
            }
            else return Task.FromException(new Exception($"Couldn't update since key: {key} doesn't exist"));
        }
    }
}