using APILibrary.Data;
using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using APILibrary.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly SmartLedgerDbContext db;
        public AuthRepository(SmartLedgerDbContext db)
        {
         this.db = db;   
        }

   

        public async Task<User?> GetUserByEmailIdAsync(string email)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }



        public async Task<User> AddUserToDatabase(User user)
        {
            await db.Users.AddAsync(user);
            return user;
        }



        public async Task<int> SaveChangesAsync()
        {
            return await db.SaveChangesAsync();
        }





        //====FOLLOWING CODE IS IN SERVICE LAYER=====//

        //public async Task<LoginRegisterResponseDTO?> RegisterAsync(RegisterDto dto)
        //{
        //    var alreadyExistingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);  //find not used coz find only works on PK
        //    if (alreadyExistingUser != null)
        //    {
        //        return null; //BadRequest("User already Exists , please login!!");
        //    }
        //    var userModel = new User
        //    {
        //        FirstName = dto.FirstName,
        //        LastName = dto.LastName,
        //        Email = dto.Email,
        //        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        //    };
        //    var ResponseDto = new LoginRegisterResponseDTO
        //    {
        //        FirstName = userModel.FirstName,
        //        LastName = userModel.LastName,
        //        Email = userModel.Email,
        //        Role = userModel.Role,
        //    };
        //    await db.Users.AddAsync(userModel);
        //    await db.SaveChangesAsync();
        //    return ResponseDto;
        //}

      
        

    }
}
