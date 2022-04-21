using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
                var commandRepository = serviceScope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var platforms = grpcClient?.GetAllPlatforms();

                if(platforms is not null)
                {
                     SeedData(commandRepository, platforms);
                }
            }
        }

        private async static void SeedData(ICommandRepository commandRepository, IEnumerable<Platform> platforms)
        {
            foreach(var platform in platforms)
            {
               if(!commandRepository.ExternalPlatformExists(platform.ExternalID))
               {
                   await commandRepository.CreatePlatformAsync(platform);
               }
            }
            
            commandRepository.SaveChanges();
        }
    }
}