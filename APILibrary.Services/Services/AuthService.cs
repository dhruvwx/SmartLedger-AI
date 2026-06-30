using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using APILibrary.Services.Interface;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService jwtService;
        private readonly IMapper mapper;
        private readonly IAuthRepository authRepo;
        public AuthService(IJwtService jwtService, IMapper mapper, IAuthRepository authRepo)
        {
            this.jwtService = jwtService;
            this.mapper = mapper;
            this.authRepo = authRepo;
        }

        public async Task<LoginRegisterResponseDTO?> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await authRepo.GetUserByEmailIdAsync(dto.Email);
            if (existingUser != null)
            {
                return null;
            }

            var newUser = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            var savedUser = await authRepo.AddUserToDatabase(newUser);
            await authRepo.SaveChangesAsync();

            var registerResponseDto = new LoginRegisterResponseDTO
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Role = newUser.Role,
            };

            return registerResponseDto;

        }
        public async Task<LoginRegisterResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var existingUser = await authRepo.GetUserByEmailIdAsync(dto.Email);
            if (existingUser == null)
            {
                return null;
            }

            var IsPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, existingUser.PasswordHash);
            if (IsPasswordValid == false)
            {
                return null;
            }

            string token = jwtService.CreateJwtToken(existingUser);

            var responseDto = mapper.Map<LoginRegisterResponseDTO>(existingUser);

            responseDto.Token = token;

            return responseDto;
        }

        public Task<LoginRegisterResponseDTO?> RegisterAsyc(RegisterDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
