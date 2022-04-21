namespace PlatformService.Data.Cache
{
    public interface ICacheService<T>
    {
        public Task SetAsync(T value);
        public Task SetAsync(IEnumerable<T> platform);
        public Task RemoveAsync(T platform);
        public Task RemovePlatformsListAsync();
        public Task UpdateAsync(T platform);

        public Task<T?> GetAsync(int Id);
        public Task<IEnumerable<T>?> GetListAsync( );
    }
}