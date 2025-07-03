using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.Questions
{
    public class CreateQuestionDto
    {
        [Required(ErrorMessage = "You Must Entre The Question Header")]

        public string Text { get; set; } = string.Empty;
        [Required(ErrorMessage = "You Must Choose Vulnerability Type to make Question")]
        public int VulnerabilityId { get; set; }
        public List<AnswerOptionDto> Options { get; set; } = new();
    }
}
