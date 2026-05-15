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
        Task<LoginRegisterResponseDTO?> RegisterAsync(RegisterDto dto);

        Task<User?> GetUserByEmailIdAsync(string email);
    }
}
