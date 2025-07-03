using System.Security.Claims;
using HackerPlatform.Core.DTOs;
using HackerPlatform.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HackerPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly IHackerPlatformService _hackerService;

        public AchievementsController(IHackerPlatformService hackerService)
        {
            _hackerService = hackerService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyAchievements()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User not logged in");

            int userId = int.Parse(userIdClaim.Value);

            var achievements = await _hackerService.GetUserAchievementsDtoAsync(userId);

            var result = achievements.Select(a => new AchievementDto
            {
                ModuleTitle = a.ModuleTitle,
                VulnerabilityType = a.VulnerabilityType,
                CreatedAt = a.CreatedAt
            });

            return Ok(result);
        }

    }
}
