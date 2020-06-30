using Automatonymous;
using Booking.SagaOrchestrator.Contracts;

namespace Booking.SagaOrchestrator
{
    internal sealed class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        public State AwaitingFlightDateConfirmedState { get; set; }
        public State AwaitingHotelDateConfirmedState { get; set; }

        public Event<IBookingCreateOrderCommand> BookingCreateOrderEvent { get; set; }
        public Event<IFlightDateConfirmedEvent> FlightDateConfirmedEvent { get; set; }
        public Event<IHotelDateConfirmedEvent> HotelDateConfirmedEvent { get; set; }
    }
}
