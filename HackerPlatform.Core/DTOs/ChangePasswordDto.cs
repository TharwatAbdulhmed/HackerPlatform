using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs
{

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "🔒 Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "🆕 New password is required")]
        [MinLength(6, ErrorMessage = "❌ Password must be at least 6 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "❌ Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }


}
