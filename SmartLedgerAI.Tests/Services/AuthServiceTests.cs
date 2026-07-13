using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using APILibrary.Services.Interface;
using APILibrary.Services.Services;
using AutoMapper;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedgerAI.Tests.Services
{
    public class AuthServiceTests
    {

        [Fact]
        public async Task LoginAsync_WithValidCredentials_CallsJwtServiceAndReturnToken()
        {
/*================
    ARRANGE - data setup and mocks
=================*/

            //request from frontend
            var loginCredentialsDto = new LoginDTO
            {
                Email = "unittest@gmail.com",
                Password = "password",
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginCredentialsDto.Password);

            //create fake user that database will return

            var user = new User
            {
                Id = 1,
                Email = loginCredentialsDto.Email,
                PasswordHash = hashedPassword,
                FirstName = "UnitName",
                LastName = "TestSurname",
                Role = "User"
            };


            var mockAuthRepo = new Mock<IAuthRepository>();
            mockAuthRepo.Setup(a => a.GetUserByEmailIdAsync(loginCredentialsDto.Email)).ReturnsAsync(user);


            var mockJwtTokenCreation = new Mock<IJwtService>();
            var fakeTokenReturnedByIJwtService = "1header.2payload-data.3signature-hmacsha256";
            mockJwtTokenCreation.Setup(j => j.CreateJwtToken(user)).Returns(fakeTokenReturnedByIJwtService);


            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<LoginRegisterResponseDTO>(user)).Returns(new LoginRegisterResponseDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = fakeTokenReturnedByIJwtService
            });


            //var mockLogger = new Mock<ILogger<AuthService>>(); 

            var authService = new AuthService(
                mockJwtTokenCreation.Object,
                mockMapper.Object,
                mockAuthRepo.Object
                );

/*================
      ACT - execute the method being tested
=================*/
        
            var result = await authService.LoginAsync(loginCredentialsDto);

/*================
      ASSERT - check if method behaves correctly
=================*/

            Assert.NotNull(result);

            Assert.Equal(fakeTokenReturnedByIJwtService, result.Token);

            Assert.Equal(user.Email, result.Email);

            mockJwtTokenCreation.Verify(j => j.CreateJwtToken(user), Times.Once);
        }









        [Fact]
        public async Task LoginAsync_WithIncorrectPassword_ReturnsNull()
        {
            //arrange
            var loginCredentialsByUserDto = new LoginDTO
            {
                Email = "unittest@gmail.com",
                Password = "WrongPassword"
            };

            var correctPassword = "CorrectPassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var user = new User
            {
                Id = 1,
                Email = loginCredentialsByUserDto.Email,
                PasswordHash = hashedPassword,
                FirstName = "Dhruv",
                LastName = "Sharma",
                Role = "User"
            };

            var mockedAuthRepo = new Mock<IAuthRepository>();
            mockedAuthRepo.Setup(a => a.GetUserByEmailIdAsync(loginCredentialsByUserDto.Email)).ReturnsAsync(user);

            var mockJwt = new Mock<IJwtService>();
            var mockMapper = new Mock<IMapper>();


            var authService = new AuthService(
                mockJwt.Object,
                mockMapper.Object,
                mockedAuthRepo.Object
                );

            //act
            var result = await authService.LoginAsync(loginCredentialsByUserDto);

            //assert
            Assert.Null(result);
            mockJwt.Verify(j => j.CreateJwtToken(It.IsAny<User>()), Times.Never());   
        }








        [Fact]
        public async Task RegisterAsync_WithValidData_HashesPasswordNotPlainText()
        {
            //arrange
            var registerUserDto = new RegisterDto
            {
                FirstName = "test",
                LastName = "Sharma",
                Email = "testsharma",
                Password = "password"
            };

            var mockAuthRepo = new Mock<IAuthRepository>();
            mockAuthRepo.Setup(ar => ar.GetUserByEmailIdAsync(registerUserDto.Email))
                        .ReturnsAsync((User?)null);


            User capturedUser = null; // container for callback to store abject that goes to database.

            mockAuthRepo.Setup(ar => ar.AddUserToDatabase(It.IsAny<User>()))
                        .Callback<User>(u => capturedUser = u)
                        .ReturnsAsync(new User { Id = 1 });


            var mockJwt = new Mock<IJwtService>();
            var mockLogger = new Mock<ILogger<IAuthService>>();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<LoginRegisterResponseDTO>(It.IsAny<User>()))
                      .Returns(new LoginRegisterResponseDTO());


            var authService = new AuthService
            (
                mockJwt.Object,
                mockMapper.Object,
                mockAuthRepo.Object
             );



            //act
            await authService.RegisterAsync(registerUserDto);

            //assert
            Assert.NotNull(capturedUser);

            Assert.StartsWith("$2a$", capturedUser.PasswordHash);

            Assert.NotEqual("password", capturedUser.PasswordHash);


        }
    }
}
  