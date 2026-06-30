using APILibrary.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Services
{
    public interface IJwtService
    {
        string CreateJwtToken(User user);
    }
}
