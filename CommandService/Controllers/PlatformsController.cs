using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        [HttpPost]
        public ActionResult TestInboundConnection()
        {
                Console.WriteLine("Testing inbound controller");

                return Ok("Testing input controller");
        }
    }
}