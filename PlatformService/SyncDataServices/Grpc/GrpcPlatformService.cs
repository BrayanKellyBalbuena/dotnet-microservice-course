using AutoMapper;
using Grpc.Core;
using PlatformService.Data.Db;
using static PlatformService.GrpcPlatform;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatformBase
    {
        private readonly IPlatformRepository platformRepository;
        private readonly IMapper mapper;

        public GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper)
        {
            this.platformRepository = platformRepository;
            this.mapper = mapper;
        }

        public override async Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response =  new PlatformResponse();
            var platforms = await platformRepository.GetAllPlatformsAsync();
             
            foreach (var platform in platforms)
            {
                response.Platform.Add(mapper.Map<GrpcPlatformModel>(platform));
            }

            return response;
        }
    }
}