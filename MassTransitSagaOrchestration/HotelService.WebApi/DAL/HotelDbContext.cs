using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelService.WebApi.DAL
{
    public class HotelDbContext : DbContext
    {
        public DbSet<Hotel> Hotels { get; set; }

        public HotelDbContext(DbContextOptions options) : base(options)
        {

        }
    }

    public class HotelDbContextFactory : IDesignTimeDbContextFactory<HotelDbContext>
    {
        public HotelDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();
            optionsBuilder.UseSqlServer("Data Source=localhost;Database=MassTransitSaga_HotelService;User ID=SA;Password=2wsx2WSX");
            return new HotelDbContext(optionsBuilder.Options);
        }
    }

    public class Hotel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid OrderId { get; set; }
        public string Country { get; set; }
        public DateTime HotelDate { get; set; }
    }
}
