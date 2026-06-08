using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Data;
namespace TransportSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RoutesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetRoutes()
        {
            var routes = await _context.Routes
                .Include(r => r.Stops)
                .ToListAsync();

            var result = routes.Select(r => new {
                r.Id,
                r.Name,
                r.Description,
                r.SchoolId,
                Stops = r.Stops.OrderBy(s => s.StopOrder).Select(s => new {
                    s.Id,
                    s.Name,
                    s.Latitude,
                    s.Longitude,
                    s.StopOrder
                }).ToList()
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetRoute(int id)
        {
            var r = await _context.Routes
                .Include(r => r.Stops)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (r == null) return NotFound();

            return Ok(new {
                r.Id,
                r.Name,
                r.Description,
                r.SchoolId,
                Stops = r.Stops.OrderBy(s => s.StopOrder).Select(s => new {
                    s.Id,
                    s.Name,
                    s.Latitude,
                    s.Longitude,
                    s.StopOrder
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreateRoute(TransportSystem.Models.Route route)
        {
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(int id, TransportSystem.Models.Route route)
        {
            if (id != route.Id) return BadRequest();
            _context.Entry(route).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null) return NotFound();
            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}