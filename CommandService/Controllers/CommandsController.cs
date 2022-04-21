using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<CommandsController> logger;

        public CommandsController(
            ICommandRepository repository,
            IMapper mapper,
            ILogger<CommandsController> logger
        )
        {
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            logger.LogInformation($"--> GetCommandsForPlatform: {platformId}");

            if (repository.PlatformExist(platformId))
            {
                var commands = repository.GetCommandsForPlatform(platformId);

                return Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
            }

            return NotFound();
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
            logger.LogInformation($"--> GetCommandForPlatform: {platformId} / {commandId}");

            if (repository.PlatformExist(platformId))
            {

                var command = await repository.GetCommandAsync(platformId, commandId);

                if (command == null)
                {
                    return NotFound();
                }

                return Ok(mapper.Map<CommandReadDto>(command));
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<CommandReadDto>> CreateCommnadForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            logger.LogInformation($"--> CreateCommandsForPlatform: {platformId}");

            if (repository.PlatformExist(platformId))
            {
                var command = mapper.Map<Command>(commandCreateDto);

                await repository.CreateCommandAsync(platformId, command);
                repository.SaveChanges();

                var commandReadDto = mapper.Map<CommandReadDto>(command);

                return CreatedAtRoute(nameof(GetCommandForPlatform),
                    new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
            }

            return NotFound();
        }
    }
}