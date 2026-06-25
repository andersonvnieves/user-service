using br.com.fiap.cloudgames.Application.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Application.Publishers
{
    public interface IUserCreatedEventPublisher
    {
        Task PublishAsync(UserCreatedEvent message);
    }
}
