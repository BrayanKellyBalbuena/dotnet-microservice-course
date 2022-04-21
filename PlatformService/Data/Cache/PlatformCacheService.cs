using Microsoft.Extensions.Caching.Distributed;
using PlatformService.Models;

namespace PlatformService.Data.Cache;
public class PlatformCacheService: ICacheService<Platform>
{
    private const string PLATFORM_IDENTIFIER = "platform:";
    private const string PLATFORMS_IDENTIFIER = "platformslist";
    private readonly ICacheRepository<Platform> platformCache;
    private readonly DistributedCacheEntryOptions cacheEntryOption;

    public PlatformCacheService(ICacheRepository<Platform> platformCache)
    {
        this.platformCache = platformCache;
        this.cacheEntryOption = new DistributedCacheEntryOptions() {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5),
        };
        
    }
    
    public async Task SetAsync(Platform platform)
    { 
        var key = GetPlatformKey(platform.Id);
        await platformCache.SetAsync(key, platform, cacheEntryOption);
    }

    public async Task SetAsync(IEnumerable<Platform> platform)
    {
        await platformCache.SetAsync(PLATFORMS_IDENTIFIER, platform, cacheEntryOption);
    }

    public async Task RemoveAsync(Platform platform)
    {
        var key = GetPlatformKey(platform.Id);
        await platformCache.RemoveAsync(key);
    }

    public async Task RemovePlatformsListAsync()
    {
        await platformCache.RemoveAsync(PLATFORMS_IDENTIFIER);
    }

    public async Task UpdateAsync(Platform platform)
    {
        var key = GetPlatformKey(platform.Id);
        await platformCache.SetAsync(key, platform, cacheEntryOption);
    }

    private string GetPlatformKey(int id)
    {
        return $"{PLATFORM_IDENTIFIER}{id}";
    }

    public async Task<Platform?> GetAsync(int Id)
    {
        var key = GetPlatformKey(Id);
        return await platformCache.GetAsync(key);
    }

    public async Task<IEnumerable<Platform>?> GetListAsync()
    {
        return await platformCache.GetListAsync(PLATFORMS_IDENTIFIER);
    }
}