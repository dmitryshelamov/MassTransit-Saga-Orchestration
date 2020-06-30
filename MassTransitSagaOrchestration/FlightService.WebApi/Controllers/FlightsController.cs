using System;
using System.Threading.Tasks;
using FlightService.WebApi.DAL;
using Microsoft.AspNetCore.Mvc;

namespace FlightService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly FlightDbContext _context;

        public FlightsController(FlightDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Flight>> Get(Guid id)
        {
            return Ok(await _context.Flights.FindAsync(id));
        }
    }
}
