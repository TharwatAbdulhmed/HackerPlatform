using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs
{
    public class CreateLevelDto
    {
        [Required(ErrorMessage ="The Level Name Is Required !! Please Enter ")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "The Description Is Required ")]

        public string Description { get; set; } = string.Empty;
    }

}
