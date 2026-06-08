using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Data;
using TransportSystem.Models;

namespace TransportSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/attendance/trip/1
        [HttpGet("trip/{tripId}")]
        public async Task<ActionResult> GetTripAttendance(int tripId)
        {
            var attendance = await _context.TripAttendances
                .Where(a => a.TripId == tripId)
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .ToListAsync();

            var result = attendance.Select(a => new {
                a.Id,
                a.TripId,
                a.StudentId,
                a.Status,
                a.NotedAt,
                Student = a.Student == null ? null : new {
                    a.Student.Id,
                    a.Student.Grade,
                    User = a.Student.User == null ? null : new {
                        a.Student.User.FullName,
                        a.Student.User.Email
                    }
                }
            });

            return Ok(result);
        }

        // POST: api/attendance
        [HttpPost]
        public async Task<ActionResult> MarkAttendance(TripAttendance attendance)
        {
            // Check if already marked
            var existing = await _context.TripAttendances
                .FirstOrDefaultAsync(a => a.TripId == attendance.TripId && a.StudentId == attendance.StudentId);

            if (existing != null)
            {
                existing.Status = attendance.Status;
                existing.NotedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(existing);
            }

            attendance.NotedAt = DateTime.UtcNow;
            _context.TripAttendances.Add(attendance);
            await _context.SaveChangesAsync();
            return Ok(attendance);
        }

        // GET: api/attendance/report
        [HttpGet("report")]
        public async Task<ActionResult> GetAttendanceReport()
        {
            var report = await _context.TripAttendances
                .Include(a => a.Trip)
                    .ThenInclude(t => t.Route)
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .OrderByDescending(a => a.NotedAt)
                .ToListAsync();

            var result = report.Select(a => new {
                a.Id,
                a.Status,
                a.NotedAt,
                Trip = a.Trip == null ? null : new {
                    a.Trip.Id,
                    a.Trip.TripDate,
                    Route = a.Trip.Route == null ? null : new {
                        a.Trip.Route.Name
                    }
                },
                Student = a.Student == null ? null : new {
                    a.Student.Id,
                    a.Student.Grade,
                    User = a.Student.User == null ? null : new {
                        a.Student.User.FullName
                    }
                }
            });

            return Ok(result);
        }
    }
}