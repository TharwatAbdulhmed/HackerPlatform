using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.Entities
{
    public class LevelProgress
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int LevelId { get; set; }
        public Level? Level { get; set; }

        public bool IsCompleted { get; set; }
    }

}
