using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.Questions
{
    public class AnswerOptionDto
    {
        [Required(ErrorMessage = "You Must add The Text Option")]

        public string Text { get; set; } = string.Empty;
        [Required(ErrorMessage = "You Must Take The Correct Text Option")]

        public bool IsCorrect { get; set; }
    }
}
