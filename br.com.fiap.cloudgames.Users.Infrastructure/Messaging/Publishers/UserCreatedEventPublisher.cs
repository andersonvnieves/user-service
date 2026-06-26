using br.com.fiap.cloudgames.Users.Application.Events;
using br.com.fiap.cloudgames.Users.Application.Publishers;
using br.com.fiap.cloudgames.Users.Infrastructure.Config;
using br.com.fiap.cloudgames.Users.Infrastructure.Messagging;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Users.Infrastructure.Messaging.Publishers
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
