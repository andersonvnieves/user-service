using Amazon.SQS;
using Amazon.SQS.Model;
using br.com.fiap.cloudgames.Users.Application.Events;
using br.com.fiap.cloudgames.Users.Application.Publishers;
using br.com.fiap.cloudgames.Users.Infrastructure.Config;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace br.com.fiap.cloudgames.Users.Infrastructure.Messaging.Publishers
{
    public class UserCreatedEventPublisher : IUserCreatedEventPublisher
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly IOptions<AwsSqsSettings> _options;
        public UserCreatedEventPublisher(IAmazonSQS sqsClient, IOptions<AwsSqsSettings> options)
        {
            _sqsClient = sqsClient;
            _options = options;
        }

        public async Task PublishAsync(UserCreatedEvent message)
        {
            var messageBody = JsonSerializer.Serialize(message);

            var request = new SendMessageRequest
            {
                QueueUrl = _options.Value.UserCreatedQueueUrl,
                MessageBody = messageBody
            };

            await _sqsClient.SendMessageAsync(request);
        }
    }
}

