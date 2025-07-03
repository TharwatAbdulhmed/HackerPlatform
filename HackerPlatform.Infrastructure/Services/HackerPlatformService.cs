using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerPlatform.Core.DTOs;
using HackerPlatform.Core.Entities;
using HackerPlatform.Core.Interfaces;

namespace HackerPlatform.Infrastructure.Services
{
    public class HackerPlatformService : IHackerPlatformService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HackerPlatformService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Level>> GetLevelsAsync()
        {
            return await _unitOfWork.Levels.GetAllAsync();
        }

        public async Task<IEnumerable<Module>> GetModulesByLevelIdAsync(int levelId)
        {
            return await _unitOfWork.Modules.FindAsync(m => m.LevelId == levelId);
        }

        public async Task ReportVulnerabilityAsync(int userId, int moduleId, string vulnerabilityType)
        {
            var achievement = new Achievement
            {
                UserId = userId,
                ModuleId = moduleId,
                VulnerabilityType = vulnerabilityType,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Achievements.AddAsync(achievement);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<AchievementDto>> GetUserAchievementsDtoAsync(int userId)
        {
            var achievements = await _unitOfWork.Achievements.FindAsync(
                a => a.UserId == userId,
                includeProperties: "Module");

            return achievements.Select(a => new AchievementDto
            {
                ModuleTitle = a.Module.Name,
                VulnerabilityType = a.VulnerabilityType,
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        //public async Task<IEnumerable<Vulnerability>> GetVulnerabilitiesByModuleIdAsync(int moduleId)
        //{
        //    return await _unitOfWork.Vulnerabilities.FindAsync(v => v.ModuleId == moduleId);
        //}

    }
}
