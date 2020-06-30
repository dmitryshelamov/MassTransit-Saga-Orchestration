using System;
using System.Threading.Tasks;
using HotelService.WebApi.DAL;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public HotelsController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Hotel>> Get(Guid id)
        {
            return Ok(await _context.Hotels.FindAsync(id));
        }
    }
}
