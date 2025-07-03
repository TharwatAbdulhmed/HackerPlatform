using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.Entities.E_Commerce
{
    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SessionToken { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
