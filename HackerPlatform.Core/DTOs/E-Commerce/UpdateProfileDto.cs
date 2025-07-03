using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.E_Commerce
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; } 
        public string? Phone { get; set; } 

        public bool IsSafe()
        {
            string[] dangerous = { "<", ">", "\"", "'", "=", "@", "#", ".", "-", "+", "*", "%", "$", "^", ":", "!", "~", "`", "\\", "|" };
            string[] dangerousEmail = { "<", ">", "\"", "'", "=", "#", "-", "+", "*", "%", "$", "^", ":", "!", "~", "`", "\\", "|" };

            return
                (FullName == null || !dangerous.Any(c => FullName.Contains(c))) &&
                (Address == null || !dangerous.Any(c => Address.Contains(c))) &&
                (Email == null || !dangerousEmail.Any(c => Email.Contains(c)));
        }
    }




}
