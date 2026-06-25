using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Application.Publishers
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string exchange, string routingKey, T message);
    }
}
