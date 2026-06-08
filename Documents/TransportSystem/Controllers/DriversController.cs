using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Data;
using TransportSystem.Models;

namespace TransportSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DriversController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetDrivers()
        {
            var drivers = await _context.Drivers
                .Include(d => d.User)
                .ToListAsync();

            var result = drivers.Select(d => new {
                d.Id,
                d.UserId,
                d.LicenseNumber,
                d.LicenseExpiry,
                d.Phone,
                d.Status,
                User = d.User == null ? null : new {
                    d.User.Id,
                    d.User.FullName,
                    d.User.Email
                }
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetDriver(int id)
        {
            var d = await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (d == null) return NotFound();

            return Ok(new {
                d.Id,
                d.UserId,
                d.LicenseNumber,
                d.LicenseExpiry,
                d.Phone,
                d.Status,
                User = d.User == null ? null : new {
                    d.User.Id,
                    d.User.FullName,
                    d.User.Email
                }
            });
        }

        [HttpPost]
        public async Task<ActionResult<Driver>> CreateDriver(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDriver), new { id = driver.Id }, driver);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(int id, Driver driver)
        {
            if (id != driver.Id) return BadRequest();
            _context.Entry(driver).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null) return NotFound();
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}