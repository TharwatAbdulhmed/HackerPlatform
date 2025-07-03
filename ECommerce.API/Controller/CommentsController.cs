
using HackerPlatform.Core.DTOs.E_Commerce;
using HackerPlatform.Core.Entities.E_Commerce;
using HackerPlatform.Infrastructure.Data;
using HackerPlatform.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
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

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                return BadRequest(new { status = false, message = $"❌ {error}" });
            }

            if (!dto.IsSafe())
                return BadRequest(new { status = false, message = "❌ Comment contains invalid characters" });

            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized();

            var comment = new Comment
            {
                Content = dto.Content,
                ProductId = dto.ProductId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "✅ Comment added successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized("User not logged in");

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound(new { message = "❌ Comment not found" });

            if (comment.UserId != user.Id)
                return BadRequest(new { message = "❌ You can only delete your own comments" });

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Comment deleted successfully" });
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetComments(int productId)
        {
            var comments = await _context.Comments
                .Where(c => c.ProductId == productId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var result = comments.Select(c => new
            {
                c.Id,
                c.Content,
                c.CreatedAt,
                User = c.User.FullName
            });

            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                return BadRequest(new { status = false, message = $"❌ {error}" });
            }

            if (!dto.IsSafe())
                return BadRequest(new { status = false, message = "❌ Comment contains invalid characters" });

            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized("User not logged in");

            var comment = await _context.Comments.FindAsync(dto.CommentId);
            if (comment == null)
                return NotFound(new { message = "❌ Comment not found" });

            if (comment.UserId != user.Id)
                return BadRequest(new { message = "❌ You can only update your own comments" });

            comment.Content = dto.Content;
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Comment updated successfully" });
        }
    }
}

