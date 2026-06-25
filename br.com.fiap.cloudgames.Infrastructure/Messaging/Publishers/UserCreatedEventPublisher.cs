using br.com.fiap.cloudgames.Application.Events;
using br.com.fiap.cloudgames.Application.Publishers;
using br.com.fiap.cloudgames.Infrastructure.Config;
using br.com.fiap.cloudgames.Infrastructure.Messagging;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Infrastructure.Messaging.Publishers
{
    public class UserCreatedEventPublisher : RabbitMqMessagePublisher, IUserCreatedEventPublisher
    {
        private readonly IOptions<RabbitMqSettings> _options;
        public UserCreatedEventPublisher(RabbitMqConnection connection, IOptions<RabbitMqSettings> options) : base(connection)
        {
            _options = options;
        }

        public async Task PublishAsync(UserCreatedEvent message)
        {
            await base.PublishAsync<UserCreatedEvent>(_options.Value.UserCreatedEvent.Exchange, _options.Value.UserCreatedEvent.RoutingKey, message);
        }
    }
}
