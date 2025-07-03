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

    public class RegisterDto
    {
        [Required(ErrorMessage = "📧 Email is required")]
        [EmailAddress(ErrorMessage = "❌ Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "🧑 First name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "❌ First name contains invalid characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "🧑 Last name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "❌ Last name contains invalid characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "📱 Phone number is required")]
        [Phone(ErrorMessage = "❌ Invalid phone number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "🔒 Password is required")]
        [MinLength(6, ErrorMessage = "❌ Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        // Optional: تأكيد كلمة المرور
        [Compare("Password", ErrorMessage = "❌ Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Checks for dangerous characters manually
        /// </summary>
        public bool IsSafe()
        {
            // الحروف/الرموز الخطيرة
            char[] dangerousChars = { '<', '>', '"', '\'', '=', '@', '#', '.', '-', '+', '*', '%', '$', '^', ':', '!', '~', '`', '\\', '|' };
            char[] dangerousCharsEmail = { '<', '>', '"', '\'', '=', '#', '-', '+', '*', '%', '$', '^', ':', '!', '~', '`', '\\', '|' };

            return
                !ContainsDangerous(FirstName, dangerousChars) &&
                !ContainsDangerous(LastName, dangerousChars) &&
                !ContainsDangerous(Email, dangerousCharsEmail);
        }

        private bool ContainsDangerous(string input, char[] dangerous)
        {
            return input.Any(c => dangerous.Contains(c));
        }
    }

}
