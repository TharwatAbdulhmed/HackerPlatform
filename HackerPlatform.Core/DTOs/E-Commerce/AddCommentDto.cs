using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HackerPlatform.Core.DTOs.E_Commerce
{

    public class AddCommentDto
    {
        [Required(ErrorMessage = "Comment cannot be empty")]
        [MinLength(3, ErrorMessage = "Comment must be at least 3 characters")]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }

        // ✅ تأمين الرموز الخطيرة
        public bool IsSafe()
        {
            string[] dangerous = { "<", ">", "\"", "'"};
            return !dangerous.Any(c => Content.Contains(c));
        }
    }

}
