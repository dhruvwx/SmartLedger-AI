using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Interface
{
    public interface IAuthRepository
    {
        //Task<LoginRegisterResponseDTO?> RegisterAsync(RegisterDto dto);  -- its business logic now in Service 

        Task<User?> GetUserByEmailIdAsync(string email);

        Task<User> AddUserToDatabase(User user);

        Task<int> SaveChangesAsync();
    }
}
