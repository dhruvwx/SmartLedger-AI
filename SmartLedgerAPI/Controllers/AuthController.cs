using APILibrary.Data;
using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SmartLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        //Dipendency injection -- creats a variable for database access
        private readonly SmartLedgerDbContext db;
        public AuthController(SmartLedgerDbContext db)
        {
            this.db = db;
        }



        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (ModelState.IsValid)
            {
                var alreadyExistingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);  //find not used coz find only works on PK
                if (alreadyExistingUser != null)
                {
                    return BadRequest("User already Exists , please login!!");
                }

                var userModel = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };
                await db.Users.AddAsync(userModel);
                await db.SaveChangesAsync();

                return Ok("User registered .... wooohooo");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var checkExistingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (checkExistingUser == null)
            {
                return BadRequest("Invalid User Email");
            }

            var checkPassword = BCrypt.Net.BCrypt.Verify(dto.Password, checkExistingUser.PasswordHash); //returns bool --(raw pass , hash pass)
            if (checkPassword == false)
            {
                return BadRequest("Incorrect password");
            }
            
            //return tokens

            return Ok("Login Successful");

        }

    }
}
