using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.Questions
{
    using System.ComponentModel.DataAnnotations;

    public class AnswerSubmissionDto
    {
        [Required(ErrorMessage = "You must select a module.")]
        [Range(1, int.MaxValue, ErrorMessage = "Module ID must be greater than zero.")]
        public int ModuleId { get; set; }

        [Required(ErrorMessage = "You must select a vulnerability.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vulnerability ID must be greater than zero.")]
        public int VulnerabilityId { get; set; }

        [Required(ErrorMessage = "You must submit at least one answer.")]
        [MinLength(1, ErrorMessage = "At least one answer is required.")]
        public List<UserAnswerDto> Answers { get; set; } = new();
    }



}
