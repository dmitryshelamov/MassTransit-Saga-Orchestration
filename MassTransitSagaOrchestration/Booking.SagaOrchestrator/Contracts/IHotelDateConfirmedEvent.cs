using System;
using MassTransit;

namespace Booking.SagaOrchestrator.Contracts
{
    public interface IHotelDateConfirmedEvent : CorrelatedBy<Guid>
    {
        public Guid OrderId { get; set; }
        public DateTime HotelDate { get; set; }
    }
}