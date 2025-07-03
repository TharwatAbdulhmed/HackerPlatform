using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.E_Commerce
{
    public class UpdateCommentDto
    {
        [Required]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Comment cannot be empty")]
        [MinLength(3, ErrorMessage = "Comment must be at least 3 characters")]
        public string Content { get; set; } = string.Empty;

        // ✅ تأمين الرموز الخطيرة
        public bool IsSafe()
        {
            string[] dangerous = { "<", ">", "\"", "'" };
            return !dangerous.Any(c => Content.Contains(c));
        }
    }

}
