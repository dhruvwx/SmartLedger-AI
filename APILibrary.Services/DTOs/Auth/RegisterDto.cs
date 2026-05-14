using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.DTOs.Auth
{
    public class RegisterDto
    {
        //what should user in the frontend give us 

        [Required]
        public string FirstName { get; set; } = string.Empty;     //string.Empty; tells not nullable if no value puts a empty string.
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]  //checks if email is in valid structure (abc@gmail.com) , rejects plain text
        public string Email { get; set; } = string.Empty;

        //will use BCrypt that stores hashed pw not orignal.
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
       
    }
}
