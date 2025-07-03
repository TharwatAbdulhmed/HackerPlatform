using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.DTOs.E_Commerce
{
    public class RegisterDto
    {
        [MaxLength(25, ErrorMessage = "The Max Length Is 25 Caracters"), MinLength(6, ErrorMessage = "The Min Length Is 6 Caracters")]
        [Required] public string FullName { get; set; } = string.Empty;
        [MaxLength(25, ErrorMessage = "The Max Length Is 25 Caracters"), MinLength(5, ErrorMessage = "The Min Length Is 5 Caracters")]
        [Required] public string Address { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, Phone] public string Phone { get; set; } = string.Empty;

        [Required, MinLength(8,ErrorMessage ="The Password Must Be 8 Caracters or more")] 
        public string Password { get; set; } = string.Empty;

        public bool IsSafe()
        {
            string[] dangerous = { "<", ">", "\"", "'" ,"=","@","#",".","-"
            ,"+","*","%","$","^",":","!","~" ,"`","\\","|"};

            string[] dangerousEmail = { "<", ">", "\"", "'" ,"=","#","-"
            ,"+","*","%","$","^",":","!","~" ,"`","\\","|"};
            return !dangerous.Any(c => FullName.Contains(c)) && !dangerous.Any(b => Address.Contains(b)) && !dangerousEmail.Any(b => Email.Contains(b));

        }
    }


}
