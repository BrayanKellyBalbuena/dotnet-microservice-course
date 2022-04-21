using PlatformService.Models;

namespace PlatformService.Data.Db
{
    public interface IPlatformRepository
    {
        Task<int> SaveChangesAsync();
        Task<bool> PlatformExistAsync(int id);
        public Task<List<Platform>> GetAllPlatformsAsync();
        public Task<Platform?> GetPlatformByIdAsync(int id);
        public Task CreatePlatformAsync(Platform platform);
        public Task UpdatePlatformAsync(Platform platform);
        public Task DeletePlatformAsync(Platform platform);
    }
}