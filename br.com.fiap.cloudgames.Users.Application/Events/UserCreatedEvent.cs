using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Users.Application.Events
{
    public class UserCreatedEvent
    {
        public Guid EventId { get; init; }
        public Guid UserId { get; init; }
        public string Name { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}
