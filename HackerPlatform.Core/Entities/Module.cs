using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.Entities
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        public int LevelId { get; set; }
        public Level? Level { get; set; }

       
        public List<Vulnerability> Vulnerabilities { get; set; } = new();
    }


}
