using HackerPlatform.Core.DTOs.E_Commerce;
using HackerPlatform.Core.Entities.E_Commerce;
using HackerPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // توليد Session ID عشوائي
        private static string GenerateSessionId()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(24))
                .Replace("=", "").Replace("+", "").Replace("/", "");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(e => e.Errors).FirstOrDefault()?.ErrorMessage;
                return BadRequest(new { status = false, message = error });
            }


            var exists = await _context.ECommerceUsers.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return BadRequest(new { status = false, message = "Email already registered" });

            if (!dto.IsSafe())
                return BadRequest(new { status = false, message = "❌ You Add invalid characters !!" });

            var user = new ECommerceUser
            {
                FullName = dto.FullName,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Balance = 4000
            };

            await _context.ECommerceUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            await SetSessionCookie(user.Id);

            return Ok(new { status = true, message = "✅ Account created successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.ECommerceUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password");

            await SetSessionCookie(user.Id);
            return Ok(new { message = "Logged in successfully" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!Request.Cookies.TryGetValue("EUser", out var sessionToken))
                return Ok(new { status = true, message = "✅ Already logged out" });

            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

            if (session != null)
            {
                _context.UserSessions.Remove(session);
                await _context.SaveChangesAsync();
            }

            // احذف الكوكي
            Response.Cookies.Delete("EUser");

            return Ok(new { status = true, message = "✅ Logged out successfully" });
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                return BadRequest(new { status = false, message = $"❌ {error}" });
            }


            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized();

            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest(new { status = false, message = "❌ Passwords do not match" });

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return BadRequest(new { status = false, message = "❌ Current password is incorrect" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "✅ Password updated successfully" });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            // ✅ نتحقق يدويًا بدل ModelState
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized();

            if (!dto.IsSafe())
                return BadRequest(new { status = false, message = "❌ You Add invalid characters !!" });

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailRegex = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
                if (!emailRegex.IsValid(dto.Email))
                    return BadRequest(new { status = false, message = "❌ Invalid Email Format" });

                var exists = await _context.ECommerceUsers
                    .AnyAsync(u => u.Email == dto.Email && u.Id != user.Id);

                if (exists)
                    return BadRequest(new { status = false, message = "❌ Email already registered" });

                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                if (dto.FullName.Length < 6 || dto.FullName.Length > 25)
                    return BadRequest(new { status = false, message = "❌ FullName must be between 6 and 25 characters" });

                user.FullName = dto.FullName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Address))
            {
                if (dto.Address.Length < 5 || dto.Address.Length > 25)
                    return BadRequest(new { status = false, message = "❌ Address must be between 4 and 25 characters" });

                user.Address = dto.Address;
            }


            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Address))
                user.Address = dto.Address;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;

            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "✅ Profile updated successfully" });
        }



        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized();

            return Ok(new
            {
                status = true,
                data = new
                {
                    user.FullName,
                    user.Email,
                    user.Address,
                    user.Phone,
                    user.Balance
                }
            });
        }

        // 🧠 استخراج المستخدم الحالي من الكوكي
        private async Task<ECommerceUser?> GetCurrentUser()
        {
            if (!Request.Cookies.TryGetValue("EUser", out var sessionId))
                return null;

            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionId);

            if (session == null)
                return null;

            return await _context.ECommerceUsers.FindAsync(session.UserId);
        }

        // 🔐 إنشاء جلسة وتخزينها مع الكوكي
        private async Task SetSessionCookie(int userId)
        {
            var sessionToken = GenerateSessionId();

            var session = new UserSession
            {
                UserId = userId,
                SessionToken = sessionToken
            };

            await _context.UserSessions.AddAsync(session);
            await _context.SaveChangesAsync();

            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(1),
                SameSite = SameSiteMode.None,  // ✅ يسمح بإرسال الكوكي من مواقع خارجية
                Secure = true                  // ✅ مطلوب مع SameSite=None
            };


            Response.Cookies.Append("EUser", sessionToken, options);
        }
    }
}
