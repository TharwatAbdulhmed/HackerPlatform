using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.Entities.E_Commerce
{
    public class ECommerceUser
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 4000;

        public List<Comment> Comments { get; set; } = new();
    }


}
