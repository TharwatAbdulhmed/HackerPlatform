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
    public class ModulesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModulesController(AppDbContext context)
        {
            _context = context;
        }

        //// ✅ Get all modules
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var modules = await _context.Modules
                .Include(m => m.Level)
                .Include(m => m.Vulnerabilities)
                .ToListAsync();

            var result = modules.Select(m => new
            {
                m.Id,
                m.Name,
                m.Url,
                Level = m.Level?.Title,
                Vulnerabilities = m.Vulnerabilities.Select(v => v.Type)
            });

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateModuleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // returns validation errors automatically

            // Check if the Level exists
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == dto.LevelId);
            if (!levelExists)
                return BadRequest(new { status = false, message = "❌ Level not found" });

            var module = new Module
            {
                Name = dto.Name,
                Url = dto.Url,
                LevelId = dto.LevelId
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "✅ Module created successfully",
                moduleId = module.Id
            });
        }



        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateModuleDto dto)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return NotFound(new { message = "Module not found" });

            // Update only if value is provided
            if (!string.IsNullOrWhiteSpace(dto.Name))
                module.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Url))
                module.Url = dto.Url;

            if (dto.LevelId.HasValue)
            {
                if (dto.LevelId <= 0)
                    return BadRequest(new { message = "Invalid LevelId. Must be greater than 0." });

                var levelExists = await _context.Levels.AnyAsync(l => l.Id == dto.LevelId.Value);
                if (!levelExists)
                    return BadRequest(new { message = "LevelId does not exist." });

                module.LevelId = dto.LevelId.Value;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Module updated successfully" });
        }

        // ✅ حذف موديول بناءً على ID
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return NotFound(new { status = false, message = "❌ Module not found" });

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "🗑️ Module deleted successfully"
            });
        }


    }

}
