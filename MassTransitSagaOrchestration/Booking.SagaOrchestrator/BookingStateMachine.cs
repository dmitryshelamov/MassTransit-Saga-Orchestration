using System;
using Automatonymous;
using Booking.SagaOrchestrator.Contracts;
using MassTransit;

namespace Booking.SagaOrchestrator
{
    internal sealed class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        public State AwaitingFlightDateConfirmedState { get; set; }
        public State AwaitingHotelDateConfirmedState { get; set; }

        public Event<IBookingCreateOrderCommand> BookingCreateOrderEvent { get; set; }
        public Event<IFlightDateConfirmedEvent> FlightDateConfirmedEvent { get; set; }
        public Event<IHotelDateConfirmedEvent> HotelDateConfirmedEvent { get; set; }

        public BookingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => BookingCreateOrderEvent,
                cc =>
                {
                    cc.CorrelateById(state => state.OrderId, context => context.Message.OrderId)
                        .SelectId(context => context.CorrelationId ?? Guid.NewGuid());
                });

            Event(() => FlightDateConfirmedEvent,
                x =>
                    x.CorrelateById(context => context.Message.CorrelationId));

            Event(() => HotelDateConfirmedEvent,
                x =>
                    x.CorrelateById(context => context.Message.CorrelationId));

            Initially(
                When(BookingCreateOrderEvent)
                    .Then(context =>
                    {
                        context.Instance.OrderId = context.Data.OrderId;
                        context.Instance.Country = context.Data.Country;
                        context.Instance.FirstName = context.Data.FirstName;
                        context.Instance.LastName = context.Data.LastName;
                    })
                    .TransitionTo(AwaitingFlightDateConfirmedState)
                    .PublishAsync(async context => await context.Init<IFlightDateRequestCommand>(new
                    {
                        CorrelationId = context.Instance.CorrelationId,
                        OrderId = context.Instance.OrderId,
                        Country = context.Instance.Country
                    })));

            During(AwaitingFlightDateConfirmedState,
                When(FlightDateConfirmedEvent)
                    .Then(context => context.Instance.FlightDate = context.Data.FlightDate)
                    .TransitionTo(AwaitingHotelDateConfirmedState)
                    .PublishAsync(async context => await context.Init<IHotelDateRequestCommand>(new
                    {
                        CorrelationId = context.Instance.CorrelationId,
                        OrderId = context.Instance.OrderId,
                        Country = context.Instance.Country
                    })));

            During(AwaitingHotelDateConfirmedState,
                When(HotelDateConfirmedEvent)
                    .Then(context => context.Instance.HotelDate = context.Data.HotelDate)
                    .Finalize());

            //  uncomment this line, if we need to remove complete transaction state 
            //  SetCompletedWhenFinalized();
        }
    }
}