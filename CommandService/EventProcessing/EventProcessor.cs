using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using CommandService.Constants;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IMapper mapper;
        private readonly ILogger<EventProcessor> logger;

        public EventProcessor(
            IServiceScopeFactory scopeFactory,
            IMapper mapper,
            ILogger<EventProcessor> logger)
        {
            this.scopeFactory = scopeFactory;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.ADD_PLATFORM_PUBLISHED:
                    await AddPlatform(message);
                    break;
                case EventType.UPDATE_PLATFORM_PUBLISHED:
                    await UpdatePlatform(message);
                    break;
                case EventType.DELETE_PLATFORM_PUBLISHED:
                    await DeletePlatform(message);
                    break;
                default:
                    break;
            }
        }

        private string DetermineEvent(string notificationMessage)
        {
            logger.LogInformation(message: "--> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType?.Event)
            {
                case EventType.ADD_PLATFORM_PUBLISHED:
                    logger.LogInformation(message: "--> Platform Published Add Event Detected");
                    return EventType.ADD_PLATFORM_PUBLISHED;
                case EventType.UPDATE_PLATFORM_PUBLISHED:
                    logger.LogInformation(message: "--> Platform Published  Update Event Detected");
                    return EventType.UPDATE_PLATFORM_PUBLISHED;
                case EventType.DELETE_PLATFORM_PUBLISHED:
                    logger.LogInformation(message: "--> Platform Published  delete Event Detected");
                        return EventType.DELETE_PLATFORM_PUBLISHED;
                default:
                    logger.LogInformation(message: "--> Could not determine the event type");
                    return EventType.UNDETERMINED;
            }
        }

        private async Task AddPlatform(string platformPublishedMessage)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                var platform = mapper.Map<Platform>(platformPublishedDto);
                if (!repo.ExternalPlatformExists(platform.Id))
                {
                    await repo.CreatePlatformAsync(platform);
                    repo.SaveChanges();
                    logger.LogInformation(message: "Platform added");
                }
                else
                {
                    logger.LogInformation(message: "-->Platform already already exist...");
                }
            }
        }

        private async Task UpdatePlatform(string platformPublishedMessage)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                var platform = mapper.Map<Platform>(platformPublishedDto);
                if (repo.ExternalPlatformExists(platform.Id))
                {
                    await repo.UpdatePlatformAsync(platform);
                    repo.SaveChanges();
                    logger.LogInformation(message: "Platform Updated");
                }
                else
                {
                    logger.LogInformation(message: "-->Platform not exist...");
                }
            }
        }

        private async Task DeletePlatform(string platformPublishedMessage)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var commandRepository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                if (commandRepository.ExternalPlatformExists(platformPublishedDto!.Id))
                {
                    var platform = await commandRepository.GetPlatformByExternalIdAsync(platformPublishedDto.Id);
                    await commandRepository.DeletePlatformAsync(platform!);
                    commandRepository.SaveChanges();
                    logger.LogInformation(message: "Platform deleted");
                }
                else
                {
                    logger.LogInformation(message: "-->Platform not exist...");
                }
            }
        }
    }
}