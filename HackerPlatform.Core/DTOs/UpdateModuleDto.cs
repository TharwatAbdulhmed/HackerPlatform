using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs
{
    public class UpdateModuleDto
    {
        // ✅ خليه Optional: يتحقق من وجوده في الـ Controller مش هنا
        public string? Name { get; set; }

        public string? Url { get; set; }

        public int? LevelId { get; set; } // nullable = not required
    }


}
