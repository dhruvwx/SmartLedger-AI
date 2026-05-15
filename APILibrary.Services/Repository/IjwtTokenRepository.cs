using APILibrary.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Repository
{
    public interface IjwtTokenRepository
    {
        //this is not async coz its on cpu not an input output
        string CreateJwtToken(User user);
    }
}
