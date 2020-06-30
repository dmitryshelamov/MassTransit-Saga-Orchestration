using System;
using System.Threading.Tasks;
using Booking.SagaOrchestrator.Contracts;
using Booking.WebApi.Dtos;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Booking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookingController(ILogger<BookingController> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateOrder(CreateBookingDto newBooking)
        {
            var orderId = Guid.NewGuid();

            _logger.LogInformation($"Generate new IBookingCreateOrderCommand message with orderId: {orderId}");

            await _publishEndpoint.Publish<IBookingCreateOrderCommand>(new
            {
                OrderId = orderId,
                Country = newBooking.Country,
                FirstName = newBooking.FirstName,
                LastName = newBooking.LastName
            });

            _logger.LogInformation($"Publish IBookingCreateOrderCommand message with orderId: {orderId}");

            return Ok(new BookingDto
            {
                OrderId = orderId,
                Country = newBooking.Country,
                FirstName = newBooking.FirstName,
                LastName = newBooking.LastName
            });
        }
    }
}
