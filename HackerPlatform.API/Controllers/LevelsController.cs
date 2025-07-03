using System.Reflection;
using HackerPlatform.Core.DTOs;
using HackerPlatform.Core.Entities;
using HackerPlatform.Core.Interfaces;
using HackerPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HackerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LevelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LevelsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all levels
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
     
            var levels = await _context.Levels.ToListAsync();
            var result = levels.Select(m => new
            {
                m.Id,
                m.Title,
                m.Description
                
            });
            return Ok(result);
        }

        // ✅ Add a new level
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLevelDto dto)
        {
            var level = new Level
            {
                Title = dto.Title,
                Description = dto.Description
            };

            _context.Levels.Add(level);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Level created", level.Id });
        }

        // ✅ حذف مستوى بناءً على ID
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var level = await _context.Levels.FindAsync(id);
            if (level == null)
                return NotFound(new { status = false, message = "❌ Level not found" });

            _context.Levels.Remove(level);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "🗑️ Level deleted successfully"
            });
        }

    }

}
