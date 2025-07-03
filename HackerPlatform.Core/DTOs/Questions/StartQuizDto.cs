using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.Questions
{
    public class StartQuizDto
    {
        [Required(ErrorMessage = "You Must Entre The Model to Start Quiz")]

        public int ModuleId { get; set; }
        [Required(ErrorMessage = "You Must Entre The VulnerabilityType to Start Quiz")]
        public string VulnerabilityType { get; set; } = string.Empty;
    }
}
