using br.com.fiap.cloudgames.Application.Publishers;
using br.com.fiap.cloudgames.Infrastructure.Messagging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace br.com.fiap.cloudgames.Infrastructure.Messaging.Publishers
{
    public abstract class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly RabbitMqConnection _connection;

        public RabbitMqMessagePublisher(RabbitMqConnection connection)
        {
            _connection = connection;
        }
        public async Task PublishAsync<T>(string exchange, string routingKey, T message)
        {
            var conn = _connection.Connection;

            var channel = await conn.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: routingKey,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                body: body);
        }    
    }
}
