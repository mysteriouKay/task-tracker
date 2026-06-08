using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Data;
using TransportSystem.Models;

namespace TransportSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var result = payments.Select(p => new {
                p.Id,
                p.StudentId,
                p.Amount,
                p.PaymentDate,
                p.PaymentMethod,
                p.Term,
                p.Notes,
                Student = p.Student == null ? null : new {
                    p.Student.Id,
                    p.Student.Grade,
                    User = p.Student.User == null ? null : new {
                        p.Student.User.FullName,
                        p.Student.User.Email
                    }
                }
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return Ok(payment);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Payment deleted." });
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult> GetStudentPayments(int studentId)
        {
            var payments = await _context.Payments
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
            return Ok(payments);
        }

        [HttpGet("summary")]
        public async Task<ActionResult> GetSummary()
        {
            var totalCollected = await _context.Payments.SumAsync(p => p.Amount);
            var totalStudents = await _context.Students.CountAsync();
            var studentsPaid = await _context.Payments
                .Select(p => p.StudentId)
                .Distinct()
                .CountAsync();

            return Ok(new {
                totalCollected,
                totalStudents,
                studentsPaid,
                studentsUnpaid = totalStudents - studentsPaid
            });
        }

        [HttpGet("/api/feestructures")]
        public async Task<ActionResult> GetFeeStructures()
        {
            var fees = await _context.FeeStructures
                .Include(f => f.School)
                .ToListAsync();
            return Ok(fees);
        }

        [HttpPost("/api/feestructures")]
        public async Task<ActionResult> CreateFeeStructure(FeeStructure fee)
        {
            _context.FeeStructures.Add(fee);
            await _context.SaveChangesAsync();
            return Ok(fee);
        }
    }
}