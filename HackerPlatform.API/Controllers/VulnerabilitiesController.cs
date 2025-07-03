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
    public class VulnerabilitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VulnerabilitiesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ 1. Get all vulnerabilities (for dropdowns or admin panels)
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var vulnerabilities = await _context.Vulnerabilities
                .Select(v => new
                {
                    v.Id,
                    v.Type,
                    v.Description,
                    v.ModuleId
                })
                .ToListAsync();

            return Ok(new
            {
                status = true,
                message = "✅ All vulnerabilities fetched successfully",
                data = vulnerabilities
            });
        }

        // ✅ 2. Get vulnerabilities by module ID
        [HttpGet("by-module/{moduleId}")]
        public async Task<IActionResult> GetByModule(int moduleId)
        {
            var vulnerabilities = await _context.Vulnerabilities
                .Where(v => v.ModuleId == moduleId)
                .Select(v => new
                {
                    v.Id,
                    v.Type,
                    v.Description
                })
                .ToListAsync();

            return Ok(new
            {
                status = true,
                message = $"✅ Vulnerabilities for module {moduleId} retrieved",
                data = vulnerabilities
            });
        }

        // ✅ 3. Create a new vulnerability linked to a module
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateVulnerabilityDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { status = false, message = "❌ Invalid data", errors = ModelState });

            var moduleExists = await _context.Modules.AnyAsync(m => m.Id == dto.ModuleId);
            if (!moduleExists)
                return BadRequest(new { status = false, message = "❌ Module not found" });

            var vulnerability = new Vulnerability
            {
                Type = dto.Type,
                Description = dto.Description,
                ModuleId = dto.ModuleId
            };

            _context.Vulnerabilities.Add(vulnerability);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "✅ Vulnerability created successfully",
                id = vulnerability.Id
            });
        }
        // ✅ 4. تحديث ثغرة معينة
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVulnerabilityDto dto)
        {
            var vuln = await _context.Vulnerabilities.FindAsync(id);
            if (vuln == null)
                return NotFound(new { status = false, message = "❌ Vulnerability not found" });

            // Update if provided
            if (!string.IsNullOrWhiteSpace(dto.Type))
                vuln.Type = dto.Type;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                vuln.Description = dto.Description;

            if (dto.ModuleId.HasValue)
            {
                if (dto.ModuleId <= 0)
                    return BadRequest(new { status = false, message = "❌ Invalid ModuleId" });

                var moduleExists = await _context.Modules.AnyAsync(m => m.Id == dto.ModuleId.Value);
                if (!moduleExists)
                    return BadRequest(new { status = false, message = "❌ Module not found" });

                vuln.ModuleId = dto.ModuleId.Value;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "✅ Vulnerability updated successfully"
            });
        }

        // ✅ 5. حذف ثغرة بناءً على ID
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var vuln = await _context.Vulnerabilities.FindAsync(id);
            if (vuln == null)
                return NotFound(new { status = false, message = "❌ Vulnerability not found" });

            _context.Vulnerabilities.Remove(vuln);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "🗑️ Vulnerability deleted successfully"
            });
        }

    }

}
