using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.E_Commerce
{
    public class ContactMessageDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Phone number is invalid")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [MinLength(5, ErrorMessage = "Message must be at least 5 characters")]
        public string Message { get; set; } = string.Empty;

        public bool IsSafe()
        {
            string[] dangerous = { "<", ">", "\"", "'", "=", "@", "#", ".", "-", "+", "*", "%", "$", "^", ":", "!", "~", "`", "\\", "|" };
            string[] dangerousEmail = { "<", ">", "\"", "'", "=", "#", "-", "+", "*", "%", "$", "^", ":", "!", "~", "`", "\\", "|" };

            return
                (Name == null || !dangerous.Any(c => Name.Contains(c))) &&
                (Message == null || !dangerous.Any(c => Message.Contains(c))) &&
                (Email == null || !dangerousEmail.Any(c => Email.Contains(c)));
        }
    }
}
