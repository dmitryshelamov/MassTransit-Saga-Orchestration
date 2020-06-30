using System;
using System.Threading.Tasks;
using Booking.SagaOrchestrator.Contracts;
using HotelService.WebApi.DAL;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HotelService.WebApi.Consumers
{
    public class BookingConsumer : IConsumer<IHotelDateRequestCommand>
    {
        private readonly ILogger<BookingConsumer> _logger;
        private readonly HotelDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookingConsumer(ILogger<BookingConsumer> logger, HotelDbContext context, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IHotelDateRequestCommand> context)
        {
            var message = context.Message;

            _logger.LogInformation($"Receive IHotelDateRequestCommand message with orderId: {message.OrderId}");

            var hotel = new Hotel
            {
                OrderId = message.OrderId,
                Country = message.Country,
                HotelDate = DateTime.Now
            };
            _context.Add(hotel);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Save order with orderId: {message.OrderId} to local storage");

            await _publishEndpoint.Publish<IHotelDateConfirmedEvent>(new
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = context.Message.OrderId,
                FligHotelDatehtDate = hotel.HotelDate
            });

            _logger.LogInformation($"Publish IHotelDateConfirmedEvent with orderId: {message.OrderId}");
        }
    }
}
