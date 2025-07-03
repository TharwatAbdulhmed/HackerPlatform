using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.Questions
{
    using System.ComponentModel.DataAnnotations;

    public class UserAnswerDto
    {
        [Required(ErrorMessage = "You must enter the question ID.")]
        [Range(1, int.MaxValue, ErrorMessage = "Question ID must be greater than zero.")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "You must enter the selected option ID.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selected option ID must be greater than zero.")]
        public int SelectedOptionId { get; set; }
    }

}
