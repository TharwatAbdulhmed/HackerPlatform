using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs
{
    public class AchievementDto
    {
        public string ModuleTitle { get; set; } = string.Empty;
        public string VulnerabilityType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
    }

}
