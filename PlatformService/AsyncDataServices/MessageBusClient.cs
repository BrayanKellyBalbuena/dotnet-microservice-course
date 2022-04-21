using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.ISyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MessageBusClient> logger;
        private readonly IConnection connection = null!;
        private readonly IModel channel = null!;

        public MessageBusClient(IConfiguration configuration, ILogger<MessageBusClient> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQHost"],
                Port = int.Parse(configuration["RabbitMQPort"])
            };
            try
            {
                this.connection = factory.CreateConnection();
                this.channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: configuration["RabbitMQExchange"], type: ExchangeType.Fanout);

                connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                logger.LogInformation("--> Connected to MessageBus");

            }
            catch (Exception ex)
            {
                logger.LogInformation($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        public void PublishPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (connection.IsOpen)
            {
                logger.LogInformation("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                logger.LogInformation("--> RabbitMQ connectionis closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: configuration["RabbitMQExchange"],
                            routingKey: "",
                            basicProperties: null,
                            body: body);
            logger.LogInformation($"--> We have sent {message}");
        }

        public void Dispose()
        {
           logger.LogInformation("MessageBus Disposed");
            if (channel.IsOpen)
            {
                channel.Close();
                connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            logger.LogInformation("--> RabbitMQ Connection Shutdown");
        }
    }
}