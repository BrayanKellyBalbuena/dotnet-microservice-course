using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        bool SaveChanges();
        public IEnumerable<Platform> GetAllPlatforms();
        public Platform GetPlatformById(int id);
        public void CreatePlatform(Platform platform);

    }
}