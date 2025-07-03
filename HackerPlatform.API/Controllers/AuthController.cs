using System.Security.Claims;
using HackerPlatform.Core.DTOs;
using HackerPlatform.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HackerPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public AuthController(IAuthService auth, IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _auth = auth;
            _uow = uow;
            _currentUser = currentUser;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 1. Model validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    status = false,
                    message = "❌ Validation failed",
                    errors
                });
            }

            // 2. Custom input sanitization check
            if (!dto.IsSafe())
            {
                return BadRequest(new
                {
                    status = false,
                    message = "❌ Your input contains unsafe characters!"
                });
            }

            // 3. Check if email already exists
            var existingUser = await _uow.Users.GetAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "❌ Email already registered"
                });
            }

            // 4. Register user
            var token = await _auth.RegisterAsync(dto);
            if (token == null)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "❌ Registration failed"
                });
            }

            // 5. Success
            return Ok(new
            {
                status = true,
                message = "✅ Registered successfully",
                token
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // 1. Validation check
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    status = false,
                    message = "❌ Validation failed",
                    errors
                });
            }

            // 2. Custom sanitization
            if (!dto.IsSafe())
            {
                return BadRequest(new
                {
                    status = false,
                    message = "❌ Email contains unsafe characters"
                });
            }

            // 3. Try login
            var token = await _auth.LoginAsync(dto);
            if (token == null)
            {
                return Unauthorized(new
                {
                    status = false,
                    message = "❌ Invalid email or password"
                });
            }

            // 4. Success
            return Ok(new
            {
                status = true,
                message = "✅ Logged in successfully",
                token
            });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = _currentUser.GetUserId(); // حسب الـ JWT
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { status = false, message = "❌ User not found" });

            var result = new
            {
                status = true,
                message = "✅ Profile fetched successfully",
                data = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Phone
                }
            };

            return Ok(result);
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(_currentUser.GetUserId());
            if (user == null)
                return NotFound(new { status = false, message = "❌ User not found" });

            // تحقق من أن المستخدم أرسل على الأقل قيمة واحدة
            if (string.IsNullOrWhiteSpace(dto.FirstName) &&
                string.IsNullOrWhiteSpace(dto.LastName) &&
                string.IsNullOrWhiteSpace(dto.Email) &&
                string.IsNullOrWhiteSpace(dto.Phone))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "❌ You must provide at least one field to update"
                });
            }

            if (!dto.IsSafe())
            {
                return BadRequest(new
                {
                    status = false,
                    message = "❌ You entered invalid data (special characters or phone number)"
                });
            }


            // تحديث الإيميل مع التحقق
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var isValidEmail = System.Text.RegularExpressions.Regex.IsMatch(
                    dto.Email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );

                if (!isValidEmail)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "❌ Invalid email format"
                    });
                }

                // تحقق من أن الإيميل غير مستخدم من مستخدم آخر
                var existingUser = await _uow.Users.GetAsync(u =>
                    u.Email == dto.Email && u.Id != user.Id);

                if (existingUser != null)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "❌ This email is already in use"
                    });
                }

                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;

            _uow.Users.Update(user);
            await _uow.CompleteAsync();

            return Ok(new
            {
                status = true,
                message = "✅ Profile updated successfully"
            });
        }



        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    status = false,
                    message = "❌ Validation failed",
                    errors
                });
            }

            var user = await _uow.Users.GetByIdAsync(_currentUser.GetUserId());
            if (user == null)
                return Unauthorized(new { status = false, message = "❌ User not found" });

            var isCorrect = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
            if (!isCorrect)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "❌ Current password is incorrect"
                });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _uow.Users.Update(user);
            await _uow.CompleteAsync();

            return Ok(new
            {
                status = true,
                message = "✅ Password changed successfully"
            });
        }




    }
}
