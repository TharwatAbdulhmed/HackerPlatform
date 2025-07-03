using System.Text.RegularExpressions;
using HackerPlatform.Core.DTOs.E_Commerce;
using HackerPlatform.Core.Entities.E_Commerce;
using HackerPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        // 🧠 استخراج SessionToken والمستخدم المرتبط به
        private async Task<(string? sessionId, ECommerceUser? user)> GetCurrentUser()
        {
            if (!Request.Cookies.TryGetValue("EUser", out var sessionId))
                return (null, null);

            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionId);

            if (session == null)
                return (sessionId, null);

            var user = await _context.ECommerceUsers.FindAsync(session.UserId);
            return (sessionId, user);
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ContactMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                return BadRequest(new { status = false, message = firstError ?? "Invalid input" });
            }

            var (sessionId, user) = await GetCurrentUser();
            if (sessionId == null)
                return Unauthorized(new { status = false, message = "❌ User not authenticated" });

            if (!dto.IsSafe())
                return BadRequest(new { status = false, message = "❌ You Add invalid characters !!" });

            var message = new ContactMessage
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Message = dto.Message,
                UserIdentifier = sessionId // نخزن SessionToken هنا
            };

            await _context.ContactMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "✅ Message sent successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyMessages()
        {
            var (sessionId, _) = await GetCurrentUser();
            if (sessionId == null)
                return Unauthorized(new { status = false, message = "❌ Unauthorized" });

            var messages = await _context.ContactMessages
                .Where(m => m.UserIdentifier == sessionId)
                .ToListAsync();

            return Ok(new { status = true, data = messages });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContactMessageDto dto)
        {
            var (sessionId, _) = await GetCurrentUser();
            if (sessionId == null)
                return Unauthorized(new { status = false, message = "❌ Unauthorized" });

            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
                return NotFound(new { status = false, message = "❌ Message not found" });

            if (message.UserIdentifier != sessionId)
                return Forbid();

            if (!dto.IsSafe())
                return BadRequest(new { status = false, message = "❌ You added invalid characters!" });

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (dto.Name.Length < 3 || dto.Name.Length > 30)
                    return BadRequest(new { status = false, message = "❌ Name must be between 3 and 30 characters." });

                message.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    return BadRequest(new { status = false, message = "❌ Invalid email format." });

                message.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                if (dto.Phone.Length < 7 || dto.Phone.Length > 15)
                    return BadRequest(new { status = false, message = "❌ Phone must be between 7 and 15 digits." });

                message.Phone = dto.Phone;
            }

            if (!string.IsNullOrWhiteSpace(dto.Message))
            {
                if (dto.Message.Length < 10 || dto.Message.Length > 300)
                    return BadRequest(new { status = false, message = "❌ Message must be between 10 and 300 characters." });

                message.Message = dto.Message;
            }

            await _context.SaveChangesAsync();
            return Ok(new { status = true, message = "✅ Message updated successfully" });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (sessionId, _) = await GetCurrentUser();
            if (sessionId == null)
                return Unauthorized(new { status = false, message = "❌ Unauthorized" });

            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
                return NotFound(new { status = false, message = "❌ Message not found" });

            if (message.UserIdentifier != sessionId)
                return Forbid();

            _context.ContactMessages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "✅ Message deleted successfully" });
        }
    }
}
