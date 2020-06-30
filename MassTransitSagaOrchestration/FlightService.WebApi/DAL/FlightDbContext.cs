using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlightService.WebApi.DAL
{
    public class FlightDbContext : DbContext
    {
        public DbSet<Flight> Flights { get; set; }

        public FlightDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }

    public class FlightDbContextFactory : IDesignTimeDbContextFactory<FlightDbContext>
    {
        public FlightDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FlightDbContext>();
            optionsBuilder.UseSqlServer("Data Source=localhost;Database=MassTransitSaga_FlightService;User ID=SA;Password=2wsx2WSX");
            return new FlightDbContext(optionsBuilder.Options);
        }
    }

    public class Flight
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid OrderId { get; set; }
        public string Country { get; set; }
        public DateTime FlightDate { get; set; }
    }
}
