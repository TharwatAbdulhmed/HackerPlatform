using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs
{
    public class CreateModuleDto
    {
        [Required(ErrorMessage = "Module name is required")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Module URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string Url { get; set; }

        [Required(ErrorMessage = "LevelId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "LevelId must be greater than 0")]
        public int LevelId { get; set; }
    }

}
