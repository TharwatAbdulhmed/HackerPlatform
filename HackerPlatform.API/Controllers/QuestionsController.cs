
using HackerPlatform.Core.DTOs.Questions;
using HackerPlatform.Core.Entities.Questions;
using HackerPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HackerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")] 
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ إضافة سؤال
        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromBody] CreateQuestionDto dto)
        {
            var vuln = await _context.Vulnerabilities.FindAsync(dto.VulnerabilityId);
            if (vuln == null) return NotFound("Vulnerability not found");

            var question = new Question
            {
                Text = dto.Text,
                VulnerabilityId = dto.VulnerabilityId,
                Options = dto.Options.Select(o => new AnswerOption
                {
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Question added successfully" });
        }

        // ✅ عرض الأسئلة حسب Vulnerability
        [HttpGet("{vulnId}")]
        public async Task<IActionResult> GetByVulnerability(int vulnId)
        {
            var questions = await _context.Questions
                .Where(q => q.VulnerabilityId == vulnId)
                .Include(q => q.Options)
                .ToListAsync();

            var result = questions.Select(q => new
            {
                q.Id,
                q.Text,
                q.VulnerabilityId,
                Options = q.Options.Select(o => new {
                    o.Id,
                    o.Text,
                    o.IsCorrect
                })
            });

            return Ok(result);
        }

        // ✅ حذف سؤال
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Question deleted." });
        }
    }

}
