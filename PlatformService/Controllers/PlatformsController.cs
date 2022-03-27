using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[Controller]")]
    [ApiController()]
    public class PlatformsController : ControllerBase 
    {
        private readonly IPlatformRepo repository;
        private readonly IMapper mapper;
        private readonly ICommandDataClient commandClient;

        public PlatformsController(
            IPlatformRepo repo,
            IMapper mapper,
            ICommandDataClient commandClient)
            {
                this.repository = repo;
                this.mapper = mapper;
                this.commandClient = commandClient;
            }
            [HttpGet()]
            public ActionResult<IEnumerable<PlatformReadDto>> GetPlatform() 
            {
                Console.WriteLine("--> Getting platform Items");

                var platformItems = repository.GetAllPlatforms();

                return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
            } 

            [HttpGet("{id}", Name = "GetPlatformById")]
            public ActionResult<PlatformReadDto> GetPlatformById(int id)
            {
                var platformItem = repository.GetPlatformById(id);

                if (platformItem is not null)
                {
                    return Ok(mapper.Map<PlatformReadDto>(platformItem));
                }

                return NotFound();
            }

            [HttpPost]
            public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto) 
            {
                if(platformCreateDto is not null)
                {
                    var platform = mapper.Map<Platform>(platformCreateDto);
                    repository.CreatePlatform(platform);
                    repository.SaveChanges();

                    var platformReadDto = mapper.Map<PlatformReadDto>(platform);

                    try
                    {
                        await commandClient.SendPlatformToCommand(platformReadDto);
                    }
                    catch (System.Exception ex)
                    {
                        
                        Console.WriteLine($"Error sending : {ex.Message}");
                    }

                    return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);
                }

                return BadRequest(platformCreateDto);
            }
    }

    
}