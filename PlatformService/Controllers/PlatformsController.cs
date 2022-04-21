using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformService.Constants;
using PlatformService.Data.Cache;
using PlatformService.Data.Db;
using PlatformService.Dtos;
using PlatformService.ISyncDataServices;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[Controller]")]
    [ApiController()]
    public class PlatformsController : ControllerBase
    {
        private readonly ILogger<PlatformsController> logger;
        private readonly IPlatformRepository platfomRepository;
        private readonly IMapper mapper;
        private readonly ICommandDataClient commandClient;
        private readonly IMessageBusClient messageBusClient;
        private readonly ICacheService<Platform> platformCacheService;

        public PlatformsController(
            ILogger<PlatformsController> logger,
            IPlatformRepository repo,
            IMapper mapper,
            ICommandDataClient commandClient,
            IMessageBusClient messaBusClient,
            ICacheService<Platform> platformCacheService)
        {
            this.logger = logger;
            this.platfomRepository = repo;
            this.mapper = mapper;
            this.commandClient = commandClient;
            this.messageBusClient = messaBusClient;
            this.platformCacheService = platformCacheService;
        }
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatform()
        {
            var platformsFromCache = await platformCacheService.GetListAsync();

            if (platformsFromCache == null)
            {
                var platformItems = await platfomRepository.GetAllPlatformsAsync();
                await platformCacheService.SetAsync(platformItems);

                logger.LogInformation("--> Getting platform Items from database");
                return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
            }
            else
            {
                logger.LogInformation("--> Getting platform Items from cache");
                return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platformsFromCache));
            }
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public async Task<ActionResult<PlatformReadDto>> GetPlatformById(int id)
        {
            var platformCache = await platformCacheService.GetAsync(id);

            if (platformCache == null)
            {
                var platformItem = await platfomRepository.GetPlatformByIdAsync(id);

                if (platformItem is not null)
                {
                    await platformCacheService.SetAsync(platformItem);

                    return Ok(mapper.Map<PlatformReadDto>(platformItem));
                }

                return NotFound();
            }
            else
            {
                logger.LogInformation($"Platform id:{platformCache.Id} from cache");
                return Ok(mapper.Map<PlatformReadDto>(platformCache));
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> PostPlatform(PlatformCreateDto platformCreateDto)
        {
            if (platformCreateDto is not null)
            {
                var platform = mapper.Map<Platform>(platformCreateDto);

                await platfomRepository.CreatePlatformAsync(platform);
                await platfomRepository.SaveChangesAsync();
                await platformCacheService.SetAsync(platform);

                var platformReadDto = mapper.Map<PlatformReadDto>(platform);

                // Send Async Message
                var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = EventType.ADD_PLATFORM_PUBLISHED;
                messageBusClient.PublishPlatform(platformPublishedDto);

                return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
            }

            return BadRequest(platformCreateDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlatform(int id, PlatformUpdateDto platformUpdateDto)
        {
            if (id != platformUpdateDto.Id)
            {
                return BadRequest();
            }

            var platformExist = await platfomRepository.PlatformExistAsync(id);

            if (platformExist)
            {
                var platform = mapper.Map<Platform>(platformUpdateDto);
                await platfomRepository.UpdatePlatformAsync(platform);
                await platfomRepository.SaveChangesAsync();
                await platformCacheService.UpdateAsync(platform);

                var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platformUpdateDto);
                platformPublishedDto.Event = EventType.UPDATE_PLATFORM_PUBLISHED;
                messageBusClient.PublishPlatform(platformPublishedDto);

                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlatform(int id)
        {
            var platformExist = await platfomRepository.PlatformExistAsync(id);

            if (platformExist)
            {
                var platform = await platfomRepository.GetPlatformByIdAsync(id);
                await platfomRepository.DeletePlatformAsync(platform!);
                await platfomRepository.SaveChangesAsync();
                await platformCacheService.RemoveAsync(platform!);

                var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platform);
                platformPublishedDto.Event = EventType.DELETE_PLATFORM_PUBLISHED;
                messageBusClient.PublishPlatform(platformPublishedDto);

                return NoContent();
            }
            return NotFound();
        }

    }
}