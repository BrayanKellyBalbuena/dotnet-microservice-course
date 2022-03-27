
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext context;
        private readonly DbSet<Platform> platformDbSet;

        public PlatformRepo(AppDbContext context)
        {
            this.context = context;
            this.platformDbSet = context.Platforms;
        }
        public void CreatePlatform(Platform platform)
        {
            if (platform is not null)
            {
                platformDbSet.Add(platform);
            }
            else 
            {
                throw new ArgumentException(nameof(platform));
            }
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return platformDbSet.ToList();
        }

        public Platform GetPlatformById(int id)
        {
            return platformDbSet.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (context.SaveChanges() >= 0);
        }
    }
}