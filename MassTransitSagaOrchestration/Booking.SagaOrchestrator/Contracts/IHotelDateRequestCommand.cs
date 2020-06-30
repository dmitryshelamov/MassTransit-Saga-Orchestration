using System;
using MassTransit;

namespace Booking.SagaOrchestrator.Contracts
{
    public interface IHotelDateRequestCommand : CorrelatedBy<Guid>
    {
        public Guid OrderId { get; set; }
        public string Country { get; set; }
    }
}