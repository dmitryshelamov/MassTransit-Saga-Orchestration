using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.SagaOrchestrator
{
    class MassTransitHostConfiguration
    {
        public string RabbitMQAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
    }
}
