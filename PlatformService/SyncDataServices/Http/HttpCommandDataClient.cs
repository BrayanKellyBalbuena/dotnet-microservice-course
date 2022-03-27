using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
           this.httpClient = httpClient;
           this.configuration = configuration;
        }
        public async Task SendPlatformToCommand(PlatformReadDto platformDto)
        {
            var stringContent = new StringContent
            (
                JsonSerializer.Serialize(platformDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync($"{configuration["CommandService"]}", stringContent);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("Sync post to command service was success");
            }
            else 
            {
                Console.WriteLine("Sync post to command service failed");
            }
        }
    }
}