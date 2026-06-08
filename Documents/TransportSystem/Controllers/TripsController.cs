using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Data;
using TransportSystem.Models;

namespace TransportSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TripsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetTrips()
        {
            var trips = await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver).ThenInclude(d => d.User)
                .ToListAsync();

            var result = trips.Select(t => new {
                t.Id,
                t.RouteId,
                t.VehicleId,
                t.DriverId,
                t.TripDate,
                t.StartTime,
                t.EndTime,
                t.Status,
                t.ScheduledTime,
                Route = t.Route == null ? null : new {
                    t.Route.Id,
                    t.Route.Name
                },
                Vehicle = t.Vehicle == null ? null : new {
                    t.Vehicle.Id,
                    t.Vehicle.PlateNumber,
                    t.Vehicle.Model
                },
                Driver = t.Driver == null ? null : new {
                    t.Driver.Id,
                    User = t.Driver.User == null ? null : new {
                        t.Driver.User.Id,
                        t.Driver.User.FullName
                    }
                }
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetTrip(int id)
        {
            var t = await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (t == null) return NotFound();

            return Ok(new {
                t.Id,
                t.RouteId,
                t.VehicleId,
                t.DriverId,
                t.TripDate,
                t.StartTime,
                t.EndTime,
                t.Status,
                t.ScheduledTime,
                Route = t.Route == null ? null : new {
                    t.Route.Id,
                    t.Route.Name
                },
                Vehicle = t.Vehicle == null ? null : new {
                    t.Vehicle.Id,
                    t.Vehicle.PlateNumber,
                    t.Vehicle.Model
                },
                Driver = t.Driver == null ? null : new {
                    t.Driver.Id,
                    User = t.Driver.User == null ? null : new {
                        t.Driver.User.Id,
                        t.Driver.User.FullName
                    }
                }
            });
        }

        [HttpPost]
        public async Task<ActionResult<Trip>> CreateTrip(Trip trip)
        {
            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTrip), new { id = trip.Id }, trip);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(int id, Trip trip)
        {
            if (id != trip.Id) return BadRequest();
            _context.Entry(trip).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null) return NotFound();
            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Check for late trips and notify admins
        [HttpPost("check-late")]
        public async Task<ActionResult> CheckLateTrips()
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);
            var today = DateOnly.FromDateTime(DateTime.Now);

            var lateTrips = await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .Where(t =>
                    t.Status == "scheduled" &&
                    t.TripDate == today &&
                    t.ScheduledTime != null &&
                    t.ScheduledTime < now.AddMinutes(-15)
                )
                .ToListAsync();

            if (lateTrips.Count == 0)
                return Ok(new { message = "No late trips.", count = 0 });

            var admins = await _context.Users
                .Where(u => u.Role == "admin")
                .ToListAsync();

            foreach (var trip in lateTrips)
            {
                foreach (var admin in admins)
                {
                    var alreadyNotified = await _context.Notifications
                        .AnyAsync(n => n.UserId == admin.Id &&
                                       n.Message.Contains($"Trip #{trip.Id}") &&
                                       n.Message.Contains("late"));

                    if (!alreadyNotified)
                    {
                        _context.Notifications.Add(new Notification
                        {
                            UserId = admin.Id,
                            Message = $"⚠️ LATE TRIP ALERT! Trip #{trip.Id} on Route {trip.Route?.Name ?? "N/A"} (Vehicle: {trip.Vehicle?.PlateNumber ?? "N/A"}) was scheduled at {trip.ScheduledTime} but has not started yet!",
                            IsRead = false,
                            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"{lateTrips.Count} late trip(s) found and admins notified.", count = lateTrips.Count });
        }
    }
}