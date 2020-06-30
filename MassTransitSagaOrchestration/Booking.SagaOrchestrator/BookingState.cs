using System;
using Automatonymous;

namespace Booking.SagaOrchestrator
{
    internal sealed class BookingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public Guid OrderId { get; set; }
        public string CurrentState { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? FlightDate { get; set; }
        public DateTime? HotelDate { get; set; }
    }
}