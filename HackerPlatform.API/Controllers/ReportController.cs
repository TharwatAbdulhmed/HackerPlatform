using System.Security.Claims;
using HackerPlatform.Core.DTOs.Questions;
using HackerPlatform.Core.Entities;
using HackerPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HackerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Step 1: Get quiz questions by module and vulnerability
        [HttpGet("quiz")]
        [Authorize]
        public async Task<IActionResult> GetQuizByModuleAndVulnerability([FromQuery] int moduleId, [FromQuery] int vulnerabilityId)
        {
            var vuln = await _context.Vulnerabilities
                .Include(v => v.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(v => v.Id == vulnerabilityId && v.ModuleId == moduleId);

            if (vuln == null)
                return BadRequest(new { message = "❌ This vulnerability does not exist in the specified module." });

            var questions = vuln.Questions.Select(q => new
            {
                q.Id,
                q.Text,
                Options = q.Options.Select(o => new { o.Id, o.Text })
            });

            return Ok(new
            {
                vulnerabilityType = vuln.Type,
                questions
            });
        }

        // ✅ Step 2: Submit answers and evaluate
        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> SubmitQuizAnswers([FromBody] AnswerSubmissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Unauthorized access." });

            int userId = int.Parse(userIdClaim.Value);

            var vulnerability = await _context.Vulnerabilities
                .Include(v => v.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(v =>
                    v.Id == dto.VulnerabilityId &&
                    v.ModuleId == dto.ModuleId);

            if (vulnerability == null)
                return BadRequest(new { message = "❌ Vulnerability not linked to the specified module." });

            // Validate answers
            foreach (var answer in dto.Answers)
            {
                var question = vulnerability.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null)
                    return BadRequest(new { message = $"❌ Invalid question ID: {answer.QuestionId}" });

                var selected = question.Options.FirstOrDefault(o => o.Id == answer.SelectedOptionId);
                if (selected == null || !selected.IsCorrect)
                    return BadRequest(new { message = "❌ One or more incorrect answers submitted." });
            }

            // Check for duplicate achievement
            var exists = await _context.Achievements.AnyAsync(a =>
                a.UserId == userId &&
                a.ModuleId == dto.ModuleId &&
                a.VulnerabilityType == vulnerability.Type);

            if (!exists)
            {
                _context.Achievements.Add(new Achievement
                {
                    UserId = userId,
                    ModuleId = dto.ModuleId,
                    VulnerabilityType = vulnerability.Type,
                    Description = $"User succeeded in {vulnerability.Type} quiz in module {vulnerability.Module?.Name}",
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "🎉 Congratulations! Your achievement has been recorded successfully." });
        }

        #region delete
        //// ✅ Delete quiz (questions and options) for specific module and vulnerability
        //[HttpDelete("quiz")]
        //[Authorize]
        //public async Task<IActionResult> DeleteQuiz([FromQuery] int moduleId, [FromQuery] int vulnerabilityId)
        //{
        //    // تأكد إن الثغرة مرتبطة بالموديول
        //    var vulnerability = await _context.Vulnerabilities
        //        .Include(v => v.Questions)
        //            .ThenInclude(q => q.Options)
        //        .FirstOrDefaultAsync(v => v.Id == vulnerabilityId && v.ModuleId == moduleId);

        //    if (vulnerability == null)
        //        return NotFound(new { message = "❌ No matching vulnerability found for this module." });

        //    // لو فيه أسئلة، احذف الاختيارات ثم الأسئلة
        //    foreach (var question in vulnerability.Questions)
        //    {
        //        _context.AnswerOptions.RemoveRange(question.Options);
        //    }

        //    _context.Questions.RemoveRange(vulnerability.Questions);

        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "✅ Quiz (questions and options) deleted successfully." });
        //} 
        #endregion

    }


}
