using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class LoginDto
    {
        [Required(ErrorMessage = "📧 Email is required")]
        [EmailAddress(ErrorMessage = "❌ Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "🔒 Password is required")]
        public string Password { get; set; } = string.Empty;

        public bool IsSafe()
        {
            char[] dangerousChars = { '<', '>', '"', '\'', '=', '#', '-', '+', '*', '%', '$', '^', ':', '!', '~', '`', '\\', '|' };
            return !Email.Any(c => dangerousChars.Contains(c));
        }
    }

}
