using CommandService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CommandService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext context;

        public CommandRepository(AppDbContext context)
        {
            this.context = context;
        }

        public bool SaveChanges()
        {
            return (context.SaveChanges() >= 0);
        }

        public async Task CreatePlatformAsync(Platform platform)
        {
            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            await context.Platforms.AddAsync(platform);
        }

        public Task UpdatePlatformAsync(Platform platform)
        {
            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            context.Platforms.Update(platform);

            return Task.CompletedTask;
        }

        public Task DeletePlatformAsync(Platform platform)
        {
            if(platform is null)
            {
                 throw new ArgumentNullException(nameof(platform));
            }

            context.Platforms.Remove(platform);

            return Task.CompletedTask;
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return context.Platforms;
        }


        public Task<Platform?> GetPlatformByExternalIdAsync(int id)
        {
            return context.Platforms.FirstOrDefaultAsync(p => p.ExternalID == id);
        }

        public bool PlatformExist(int platformId)
        {
            return context.Platforms.Any(p => p.Id == platformId);
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
            return context.Platforms.Any(p => p.ExternalID == externalPlatformId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return context.Commands
                .Where(c => c.PlatformID == platformId)
                .OrderBy(c => c.Platform.Name);
        }

        public Task<Command?> GetCommandAsync(int platformId, int commandId)
        {
            return context.Commands
                .Where(c => c.PlatformID == platformId && c.Id == commandId)
                .FirstOrDefaultAsync();
        }

        public async Task CreateCommandAsync(int platformId, Command command)
        {
            if (command is null)
            {
                throw new NullReferenceException(nameof(command));
            }

            command.PlatformID = platformId;
           await context.Commands.AddAsync(command);
        }
    }
}