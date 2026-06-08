using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Data;
using TransportSystem.Models;
using TransportSystem.Services;

namespace TransportSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly SmsService _sms;
        private readonly EmailService _email;

        public NotificationsController(AppDbContext context, SmsService sms, EmailService email)
        {
            _context = context;
            _sms = sms;
            _email = email;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetUserNotifications(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return Ok(notifications);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return Ok(notification);
        }

        [HttpPut("read/{id}")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();
            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok(notification);
        }

        [HttpPut("readall/{userId}")]
        public async Task<ActionResult> MarkAllAsRead(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();
            foreach (var n in notifications)
                n.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "All marked as read." });
        }

        [HttpPost("sos")]
        public async Task<ActionResult> SendSOS([FromBody] SOSRequest request)
        {
            var admins = await _context.Users
                .Where(u => u.Role == "admin")
                .ToListAsync();

            foreach (var admin in admins)
            {
                var message = $"SOS EMERGENCY! Driver {request.DriverName} on Route {request.RouteName} (Vehicle: {request.VehiclePlate}) needs immediate assistance!";
                var notification = new Notification
                {
                    UserId = admin.Id,
                    Message = $"🆘 {message}",
                    IsRead = false,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                };
                _context.Notifications.Add(notification);

                // SMS
                if (!string.IsNullOrEmpty(admin.Phone))
                    await _sms.SendSmsAsync(admin.Phone, message);

                // Email
                if (!string.IsNullOrEmpty(admin.Email))
                {
                    var emailBody = $@"
                        <h2 style='color:red'>🆘 SOS EMERGENCY ALERT</h2>
                        <p><b>Driver:</b> {request.DriverName}</p>
                        <p><b>Route:</b> {request.RouteName}</p>
                        <p><b>Vehicle:</b> {request.VehiclePlate}</p>
                        <p><b>Location:</b> {request.Latitude}, {request.Longitude}</p>
                        <p><b>Time:</b> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                        <hr/>
                        <p style='color:gray'>School Transport Management System</p>
                    ";
                    await _email.SendEmailAsync(admin.Email, admin.FullName, "🆘 SOS Emergency Alert", emailBody);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "SOS sent to all admins." });
        }
    }

    public class SOSRequest
    {
        public string DriverName { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}