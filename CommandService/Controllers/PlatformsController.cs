using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<PlatformsController> logger;

        public PlatformsController(
            ICommandRepository repository,
            IMapper mapper,
            ILogger<PlatformsController> logger
        )
        {
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            logger.LogInformation("Getting Platforms from CommandsService");
            
            var platforms = repository.GetAllPlatforms();

            return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }
    }
}