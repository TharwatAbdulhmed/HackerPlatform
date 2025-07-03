using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerPlatform.Core.DTOs;
using HackerPlatform.Core.Entities;

namespace HackerPlatform.Core.Interfaces
{
    public interface IHackerPlatformService
    {
        Task<IEnumerable<Level>> GetLevelsAsync();
        Task<IEnumerable<Module>> GetModulesByLevelIdAsync(int levelId);
        //Task<IEnumerable<Vulnerability>> GetVulnerabilitiesByModuleIdAsync(int moduleId);

        Task ReportVulnerabilityAsync(int userId, int moduleId, string vulnerabilityType);
        Task<IEnumerable<AchievementDto>> GetUserAchievementsDtoAsync(int userId);

    }
}
