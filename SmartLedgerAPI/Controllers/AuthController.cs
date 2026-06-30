using APILibrary.Data;
using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using APILibrary.Services.Interface;
using APILibrary.Services.Repository;
using APILibrary.Services.Services;
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

        ///====-- in service now====///
        //private readonly IjwtTokenRepository tokenRepository; 
        //private readonly IAuthRepository authRepository;
        //private readonly IMapper mapper;


        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;
        public AuthController(/*SmartLedgerDbContext db,*/
            IAuthService authService , ILogger<AuthController> logger 
            /*IMapper mapper , IjwtTokenRepository tokenRepository*/ )
        {
            //this.db = db;
            //this.mapper = mapper;
            //this.tokenRepository = tokenRepository;


            this.authService = authService;
            
            this.logger = logger;
        }



        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var response = await authService.RegisterAsync(dto);
            if (ModelState.IsValid)
            {
                if(response == null)
                {
                    logger.LogWarning($"{dto.Email} already exists");
                    return BadRequest("User already exist please Login");
                }
                else
                {
                    logger.LogInformation($"new user {dto.Email} registered");
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

            var response = await authService.LoginAsync(dto);
            if (response == null)
            {
                logger.LogWarning($"{dto.Email} does not exist");
                return BadRequest(dto.Email + " does not exist");
            }





            //var checkPassword = BCrypt.Net.BCrypt.Verify(dto.Password, existingUser.PasswordHash); //returns bool --(raw pass , hash pass)
            //if (checkPassword == false)
            //{
            //    logger.LogWarning($"Failed login for {dto.Email} incorrect password");
            //    return BadRequest("Incorrect Password");
            //}
            ////return tokens
            //string token = tokenRepository.CreateJwtToken(existingUser);





            logger.LogInformation($"{dto.Email} logined successfully");




            //var existingUserDtoResponse = mapper.Map<LoginRegisterResponseDTO>(existingUser);
            //existingUserDtoResponse.Token = token;




            //var ExistingUserDtoResponse = new LoginRegisterResponseDTO
            //{
            //    Email = existingUser.Email,
            //    FirstName = existingUser.FirstName,
            //    LastName = existingUser.LastName,
            //    Role = existingUser.Role
            //};



            return Ok(response);

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
