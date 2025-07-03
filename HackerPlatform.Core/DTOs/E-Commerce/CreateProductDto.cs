using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HackerPlatform.Core.DTOs.E_Commerce
{
    public class CreateProductDto
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsSafe()
        {
            char[] dangerous = { '<', '>', '"', '\'', '=', '#', '-', '+', '*', '%', '$', '^', ':', '!', '~', '`', '\\', '|' };


            return
                (Name == null || !Name.Any(c => dangerous.Contains(c))) &&
                (Description == null || !Description.Any(c => dangerous.Contains(c)));
               
        }
    }

}
