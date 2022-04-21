
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data.Db
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext context;
        private readonly DbSet<Platform> platformDbSet;

        public PlatformRepository(AppDbContext context)
        {
            this.context = context;
            this.platformDbSet = context.Platforms;
        }
        public async Task CreatePlatformAsync(Platform platform)
        {
            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            else
            {
                await platformDbSet.AddAsync(platform);
            }
        }

        public Task UpdatePlatformAsync(Platform platform)
        {
            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            else
            {
                context.Update(platform);
                return Task.CompletedTask;
            }
        }

        public Task DeletePlatformAsync(Platform platform)
        {
            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            else
            {
                platformDbSet.Remove(platform);
                return Task.CompletedTask;
            }

        }

        public Task<List<Platform>> GetAllPlatformsAsync()
        {
            return platformDbSet.ToListAsync();
        }

        public Task<Platform?> GetPlatformByIdAsync(int id)
        {
            return  platformDbSet.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public Task<bool> PlatformExistAsync(int id)
        {
            return platformDbSet.AnyAsync(plat => plat.Id == id);
        }
    }
}