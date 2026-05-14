using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.DTOs.Auth
{
    public class LoginRegisterResponseDTO
    {

        // this is what as backend we will send back

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //new user is user by default
        public string Role { get; set; } = "User";
    }
}
