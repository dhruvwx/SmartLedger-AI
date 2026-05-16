using APILibrary.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Repository
{
    public class JwtTokenRepository : IjwtTokenRepository
    {
        private readonly IConfiguration config; //for appsetting jwt
        public JwtTokenRepository(IConfiguration config)
        {
            this.config = config;
        }

        public string CreateJwtToken(User user)
        {
            var claims = new List<Claim>
           {
               new Claim(ClaimTypes.Email, user.Email),  // ClaimeTypes.x , value
               new Claim(ClaimTypes.Role , user.Role),
               new Claim(ClaimTypes.NameIdentifier , user.Id.ToString())
           };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));  //get bytes need appsetings jwt key so need configuration.

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

