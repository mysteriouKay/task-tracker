using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Data;
using TransportSystem.Models;

namespace TransportSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var result = users.Select(u => new {
                u.Id,
                u.FullName,
                u.Email,
                u.Role,
                u.SchoolId,
                u.CreatedAt
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return NotFound();
            return Ok(new {
                u.Id,
                u.FullName,
                u.Email,
                u.Role,
                u.SchoolId,
                u.CreatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id) return BadRequest();
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}