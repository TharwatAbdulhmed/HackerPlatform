using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.E_Commerce
{
    public class UpdateContactMessageDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Message { get; set; }

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
