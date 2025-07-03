using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.Entities
{
    public class Achievement
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public string Description { get; set; }
        public int ModuleId { get; set; }
        public Module? Module { get; set; }

        public string VulnerabilityType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
