using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataService
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly IEventProcessor eventProcessor;
        private readonly ILogger<MessageBusSubscriber> logger;
        private IConnection? connection;
        private IModel? channel;
        private string? queueName;

        public MessageBusSubscriber(
            IConfiguration configuration,
            IEventProcessor eventProcessor,
            ILogger<MessageBusSubscriber> logger)
        {
            this.configuration = configuration;
            this.eventProcessor = eventProcessor;
            this.logger = logger;

            InitializedRabbitMQ();
        }

        private void InitializedRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = configuration["RabbitMQHost"],
                    Port = int.Parse(configuration["RabbitMQPort"])
                };

                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: configuration["RabbitMQExchange"], type: ExchangeType.Fanout);
                queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName,
                     exchange: configuration["RabbitMQExchange"],
                     routingKey: configuration["RabbitMQRoutingKey"]
                );

                logger.LogInformation(message: "Listening on the Message Bus...");
                connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            }
            catch (Exception ex)
            {
                logger.LogError($":{ex.Message} ");
            }

        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            logger.LogInformation(message: "Connection Shutdown");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            
            try
            {
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (ModuleHandle, eventArgs) =>
                {
                    logger.LogInformation(message: "Event Received");

                    var body = eventArgs.Body;
                    var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                    eventProcessor.ProcessEvent(notificationMessage);
                };

                channel?.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Task.FromException(ex);
            }
        }

        public override void Dispose()
        {
            if (channel is not null)
            {
                if (channel.IsOpen)
                {
                    channel.Close();
                    connection?.Close();
                }
            }

            base.Dispose();
        }
    }
}