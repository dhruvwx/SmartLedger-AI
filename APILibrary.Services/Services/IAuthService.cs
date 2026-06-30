using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Services
{
    public interface IAuthService
    {
        Task<LoginRegisterResponseDTO?> RegisterAsync(RegisterDto dto);
        Task<LoginRegisterResponseDTO?> LoginAsync(LoginDTO dto);
       
    }
}
