using Azure.Messaging.ServiceBus;
using System.Text.Json;
using WasteConnect.Models;

namespace WasteConnect.Services
{
    public class CommunityAlertQueueService : IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        private const string QueueName = "community-alert-sms";

        public CommunityAlertQueueService(
            IConfiguration configuration)
        {
            var connectionString =
                configuration["ServiceBus:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Service Bus connection string is missing.");
            }

            _client = new ServiceBusClient(connectionString);

            _sender = _client.CreateSender(QueueName);
        }

        public async Task EnqueueSmsAsync(
            string phoneNumber,
            string message,
            string alertId)
        {
            var smsJob = new CommunityAlertSmsMessage
            {
                PhoneNumber = phoneNumber,
                Message = message,
                AlertId = alertId
            };

            var json = JsonSerializer.Serialize(smsJob);

            var serviceBusMessage =
                new ServiceBusMessage(json)
                {
                    ContentType = "application/json",

                    Subject = "CommunityAlertSms",

                    MessageId = Guid.NewGuid().ToString()
                };

            await _sender.SendMessageAsync(
                serviceBusMessage);
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}