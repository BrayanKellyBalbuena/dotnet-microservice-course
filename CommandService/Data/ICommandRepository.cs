using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepository
    {
        bool SaveChanges();
        IEnumerable<Platform> GetAllPlatforms();
        Task<Platform?> GetPlatformByExternalIdAsync(int id);
        Task CreatePlatformAsync(Platform platform);
        Task UpdatePlatformAsync(Platform platform);

        Task DeletePlatformAsync(Platform platform);
        bool PlatformExist(int platformId);
        bool ExternalPlatformExists(int externalPlatformId);
        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Task<Command?> GetCommandAsync(int platformId, int commandId);
        Task CreateCommandAsync(int platformId,Command command);
    }
}