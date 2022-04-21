using PlatformService.Dtos;

namespace PlatformService.ISyncDataServices
{
    public interface IMessageBusClient
    {
         void PublishPlatform(PlatformPublishedDto platformPublishedDto);
    }
}