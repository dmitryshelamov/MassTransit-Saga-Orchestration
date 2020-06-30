using System;
using MassTransit;

namespace Booking.SagaOrchestrator.Contracts
{
    public interface IFlightDateConfirmedEvent : CorrelatedBy<Guid>
    {
        public Guid OrderId { get; set; }
        public DateTime FlightDate { get; set; }
    }
}