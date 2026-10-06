using Azure.Messaging.ServiceBus;
using System.Text.Json;
using WasteConnect.Models;

namespace WasteConnect.Services
{
    public class CommunityAlertSmsWorker : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CommunityAlertSmsWorker> _logger;

        private const string QueueName = "community-alert-sms";

        public CommunityAlertSmsWorker(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger<CommunityAlertSmsWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var connectionString =
                configuration["ServiceBus:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Service Bus connection string is missing.");
            }

            _client =
                new ServiceBusClient(connectionString);

            var processorOptions =
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,

                    MaxConcurrentCalls = 5,

                    MaxAutoLockRenewalDuration =
                        TimeSpan.FromMinutes(5)
                };

            _processor =
                _client.CreateProcessor(
                    QueueName,
                    processorOptions);

            _processor.ProcessMessageAsync +=
                ProcessMessageAsync;

            _processor.ProcessErrorAsync +=
                ProcessErrorAsync;
        }


        // =====================================================
        // START SERVICE BUS WORKER
        // =====================================================

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Community Alert SMS Worker started.");

            await _processor.StartProcessingAsync(
                stoppingToken);

            try
            {
                await Task.Delay(
                    Timeout.Infinite,
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Normal application shutdown
            }
        }


        // =====================================================
        // PROCESS SMS MESSAGE
        // =====================================================

        private async Task ProcessMessageAsync(
            ProcessMessageEventArgs args)
        {
            try
            {
                var json =
                    args.Message.Body.ToString();

                var smsJob =
                    JsonSerializer.Deserialize
                        <CommunityAlertSmsMessage>(json);

                if (smsJob == null ||
                    string.IsNullOrWhiteSpace(
                        smsJob.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(
                        smsJob.Message))
                {
                    _logger.LogWarning(
                        "Invalid Community Alert SMS message. " +
                        "MessageId: {MessageId}",
                        args.Message.MessageId);

                    await args.DeadLetterMessageAsync(
                        args.Message,
                        deadLetterReason:
                            "InvalidMessage",
                        deadLetterErrorDescription:
                            "Phone number or SMS message was missing.");

                    return;
                }


                // TwilioOtpService is Scoped,
                // so create a DI scope.
                using var scope =
                    _scopeFactory.CreateScope();

                var twilioService =
                    scope.ServiceProvider
                        .GetRequiredService<TwilioOtpService>();


                // Send SMS through existing Twilio service
                await twilioService.SendSmsAsync(
                    smsJob.PhoneNumber,
                    smsJob.Message);


                // Twilio succeeded.
                // Remove message from Service Bus.
                await args.CompleteMessageAsync(
                    args.Message);


                _logger.LogInformation(
                    "Community Alert SMS sent successfully. " +
                    "AlertId: {AlertId}, MessageId: {MessageId}",
                    smsJob.AlertId,
                    args.Message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send Community Alert SMS. " +
                    "MessageId: {MessageId}. " +
                    "Service Bus will retry the message.",
                    args.Message.MessageId);

                // IMPORTANT:
                // We do NOT complete the message.
                //
                // Abandon makes it available for another
                // delivery attempt.
                await args.AbandonMessageAsync(
                    args.Message);
            }
        }


        // =====================================================
        // SERVICE BUS PROCESSOR ERRORS
        // =====================================================

        private Task ProcessErrorAsync(
            ProcessErrorEventArgs args)
        {
            _logger.LogError(
                args.Exception,
                "Azure Service Bus error. " +
                "Entity: {EntityPath}, " +
                "ErrorSource: {ErrorSource}",
                args.EntityPath,
                args.ErrorSource);

            return Task.CompletedTask;
        }


        // =====================================================
        // STOP WORKER
        // =====================================================

        public override async Task StopAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Stopping Community Alert SMS Worker.");

            await _processor.StopProcessingAsync(
                cancellationToken);

            await base.StopAsync(
                cancellationToken);
        }


        // =====================================================
        // CLEAN UP SERVICE BUS CONNECTION
        // =====================================================

        public override void Dispose()
        {
            _processor.DisposeAsync()
                .AsTask()
                .GetAwaiter()
                .GetResult();

            _client.DisposeAsync()
                .AsTask()
                .GetAwaiter()
                .GetResult();

            base.Dispose();
        }
    }
}