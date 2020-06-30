using System;
using System.Collections.Generic;
using System.Text;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Booking.SagaOrchestrator.Storage
{
    public class BookingDbContext : SagaDbContext
    {
        public BookingDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new BookingStateMap(); }
        }
    }

    public class BookingDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
    {
        public BookingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookingDbContext>();
            optionsBuilder.UseSqlServer("Data Source=localhost;Database=MassTransitSaga_BookingOrchestrator;User ID=SA;Password=2wsx2WSX");
            return new BookingDbContext(optionsBuilder.Options);
        }
    }
}
