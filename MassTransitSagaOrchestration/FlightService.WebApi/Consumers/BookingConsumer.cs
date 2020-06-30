using System;
using System.Threading.Tasks;
using Booking.SagaOrchestrator.Contracts;
using FlightService.WebApi.DAL;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightService.WebApi.Consumers
{
    public class BookingConsumer : IConsumer<IFlightDateRequestCommand>
    {
        private readonly ILogger<BookingConsumer> _logger;
        private readonly FlightDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookingConsumer(ILogger<BookingConsumer> logger, FlightDbContext context, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IFlightDateRequestCommand> context)
        {
            var message = context.Message;

            _logger.LogInformation($"Receive IFlightDateRequestCommand message with orderId: {message.OrderId}");

            var flight = new Flight
            {
                OrderId = message.OrderId,
                Country = message.Country,
                FlightDate = DateTime.Now
            };
            _context.Add(flight);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Save order with orderId: {message.OrderId} to local storage");

            int delay = 10000;
            _logger.LogInformation($"Delay {delay} ms");
            await Task.Delay(delay);

            await _publishEndpoint.Publish<IFlightDateConfirmedEvent>(new
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = context.Message.OrderId,
                FlightDate = flight.FlightDate
            });

            _logger.LogInformation($"Publish IFlightDateConfirmedEvent with orderId: {message.OrderId}");
        }
    }
}
