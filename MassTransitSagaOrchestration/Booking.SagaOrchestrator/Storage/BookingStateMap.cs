using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.SagaOrchestrator.Storage
{
    class BookingStateMap : SagaClassMap<BookingState>
    {
        protected override void Configure(EntityTypeBuilder<BookingState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
        }
    }
}
