using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Data;
using TransportSystem.Models;
namespace TransportSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .Include(s => s.School)
                .Include(s => s.Vehicle)
                .ToListAsync();
            var result = students.Select(s => new {
                s.Id,
                s.UserId,
                s.SchoolId,
                s.Grade,
                s.ParentId,
                s.VehicleId,
                User = s.User == null ? null : new {
                    s.User.Id,
                    s.User.FullName,
                    s.User.Email
                },
                School = s.School == null ? null : new {
                    s.School.Id,
                    s.School.Name
                },
                Vehicle = s.Vehicle == null ? null : new {
                    s.Vehicle.Id,
                    s.Vehicle.PlateNumber,
                    s.Vehicle.Model
                }
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetStudent(int id)
        {
            var s = await _context.Students
                .Include(s => s.User)
                .Include(s => s.School)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (s == null) return NotFound();
            return Ok(new {
                s.Id,
                s.UserId,
                s.SchoolId,
                s.Grade,
                s.ParentId,
                s.VehicleId,
                User = s.User == null ? null : new {
                    s.User.Id,
                    s.User.FullName,
                    s.User.Email
                },
                School = s.School == null ? null : new {
                    s.School.Id,
                    s.School.Name
                },
                Vehicle = s.Vehicle == null ? null : new {
                    s.Vehicle.Id,
                    s.Vehicle.PlateNumber,
                    s.Vehicle.Model
                }
            });
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.Id) return BadRequest();
            _context.Entry(student).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}