using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Data;
using TransportSystem.Models;

namespace TransportSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripLocationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TripLocationsController(AppDbContext context)
        {
            _context = context;
        }

        // Driver posts their location
        [HttpPost]
        public async Task<ActionResult> PostLocation(TripLocation location)
        {
            location.RecordedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            _context.TripLocations.Add(location);
            await _context.SaveChangesAsync();
            return Ok(location);
        }

        // Get latest location for a trip
        [HttpGet("{tripId}")]
        public async Task<ActionResult> GetLatestLocation(int tripId)
        {
            var location = await _context.TripLocations
                .Where(l => l.TripId == tripId)
                .OrderByDescending(l => l.RecordedAt)
                .FirstOrDefaultAsync();

            if (location == null) return NotFound(new { message = "No location data yet." });
            return Ok(location);
        }

        // Get all active trip locations (for admin map)
        [HttpGet("active")]
        public async Task<ActionResult> GetActiveLocations()
        {
            var activeTrips = await _context.Trips
                .Where(t => t.Status == "in_progress")
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .ToListAsync();

            var result = new List<object>();
            foreach (var trip in activeTrips)
            {
                var location = await _context.TripLocations
                    .Where(l => l.TripId == trip.Id)
                    .OrderByDescending(l => l.RecordedAt)
                    .FirstOrDefaultAsync();

                if (location != null)
                {
                    result.Add(new {
                        tripId = trip.Id,
                        route = trip.Route?.Name,
                        vehicle = trip.Vehicle?.PlateNumber,
                        latitude = location.Latitude,
                        longitude = location.Longitude,
                        recordedAt = location.RecordedAt
                    });
                }
            }

            return Ok(result);
        }
    }
}