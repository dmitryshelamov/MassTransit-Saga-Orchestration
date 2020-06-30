using System;

namespace Booking.WebApi.Dtos
{
    public class BookingDto
    {
        public Guid OrderId { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}