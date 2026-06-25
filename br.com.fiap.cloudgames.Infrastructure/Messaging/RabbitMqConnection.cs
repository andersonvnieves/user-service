using br.com.fiap.cloudgames.Infrastructure.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace br.com.fiap.cloudgames.Infrastructure.Messagging
{
    public class RabbitMqConnection : IAsyncDisposable
    {
        private readonly IConnection _connection;

        public RabbitMqConnection(IOptions<RabbitMqSettings> settings) {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri(settings.Value.URI);

            _connection = factory.CreateConnectionAsync()
                            .GetAwaiter()
                            .GetResult();
        }

        public IConnection Connection => _connection;

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
