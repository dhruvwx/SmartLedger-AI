using APILibrary.Data;
using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using APILibrary.Services.Interface;
using APILibrary.Services.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLedgerAPI.AutoMapper;
using System.Security.Claims;

namespace SmartLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        //Dipendency injection -- creats a variable for database access
        //private readonly SmartLedgerDbContext db;

        private readonly IjwtTokenRepository tokenRepository;
        private readonly IAuthRepository authRepository;
        private readonly IMapper mapper;
        public AuthController(/*SmartLedgerDbContext db,*/ 
            IAuthRepository authRepository , IMapper mapper , IjwtTokenRepository tokenRepository)
        {
            //this.db = db;
            this.authRepository = authRepository;
            this.mapper = mapper;
            this.tokenRepository = tokenRepository;
        }



        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var response = await authRepository.RegisterAsync(dto);
            if (ModelState.IsValid)
            {
                if(response == null)
                {
                    return BadRequest("User already exist please Login");
                }
                else
                {
                    return Ok(response);
                }    
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

            var existingUser = await authRepository.GetUserByEmailIdAsync(dto.Email);
            if (existingUser == null)
            {
                return BadRequest(dto.Email + " does not exist");
            }

            var checkPassword = BCrypt.Net.BCrypt.Verify(dto.Password, existingUser.PasswordHash); //returns bool --(raw pass , hash pass)
            if (checkPassword == false)
            {
                return BadRequest("Incorrect password");
            }
            //return tokens
            string token = tokenRepository.CreateJwtToken(existingUser);

            var existingUserDtoResponse = mapper.Map<LoginRegisterResponseDTO>(existingUser);
            existingUserDtoResponse.Token = token;

            //var ExistingUserDtoResponse = new LoginRegisterResponseDTO
            //{
            //    Email = existingUser.Email,
            //    FirstName = existingUser.FirstName,
            //    LastName = existingUser.LastName,
            //    Role = existingUser.Role
            //};

            return Ok(existingUserDtoResponse);

        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        public IActionResult Me()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { Email = email, Role = role }); // output is json so not E but email -- javascript uses camelCase
        }


        [HttpGet]
        [Route("Admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok("you are an admin");
        }

    }
}
