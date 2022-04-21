using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ILogger<PlatformDataClient> logger;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper,
            ILogger<PlatformDataClient> logger)
        {
            this.configuration = configuration;
            this.mapper = mapper;
            this.logger = logger;
        }
        public IEnumerable<Platform>? GetAllPlatforms()
        {
            
            logger.LogInformation($"Calling GRPC Service {configuration["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(configuration["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel); 
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return mapper.Map<IEnumerable<Platform>>(reply.Platform); 
            }
            catch (Exception ex)
            {
                logger.LogError($"Could not call Grpc Server {ex.Message}");
            }

            return null;
        }
    }
}