using System;

namespace Booking.SagaOrchestrator.Contracts
{
    public interface IBookingCreateOrderCommand
    {
        public Guid OrderId { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}