using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Infrastructure.Config
{
    public class RabbitMqSettings
    {
        public string URI { get; set; }
        public RabbitMqQueueDetailsSettings UserCreatedEvent { get; set; }
    }
}
