using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class UpdateProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public bool IsSafe()
        {
            char[] dangerous = { '<', '>', '"', '\'', '=', '#', '-', '+', '*', '%', '$', '^', ':', '!', '~', '`', '\\', '|' };
            // تحقق من صيغة رقم الموبايل (مثلاً يبدأ بـ 01 ويتكون من 11 رقم)
            bool isValidPhone = string.IsNullOrWhiteSpace(Phone) || Regex.IsMatch(Phone, @"^01[0-9]{9}$");

            return
                (FirstName == null || !FirstName.Any(c => dangerous.Contains(c))) &&
                (LastName == null || !LastName.Any(c => dangerous.Contains(c))) &&
                (Email == null || !Email.Any(c => dangerous.Contains(c))) &&
                (Phone == null || !Phone.Any(c => dangerous.Contains(c))) &&
                isValidPhone;

        }
    }


}
