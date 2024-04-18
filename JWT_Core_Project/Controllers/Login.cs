using JWT_Core_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;



namespace JWT_Core_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private IConfiguration _config;

        public LoginController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _config = configuration;
        }

        private string GenerateToken(Users users)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], null, expires: DateTime.Now.AddHours(1), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            // var user = await _dbContext.users.FirstOrDefaultAsync(u => u.username == username && u.password == password && u.isActive == true);
            var user = await _dbContext.users.FirstOrDefaultAsync(u => u.username == username /* && u.password == password */ && u.isActive == true);

            if (user != null)
            {
               // Verify the password
                var passwordHasher = new PasswordHasher<Users>();
                var result = passwordHasher.VerifyHashedPassword(user, user.password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    var token = GenerateToken(user);
                
                    Response.StatusCode = 200;
                    return Ok(new { token = token });
                }
            }

            // If authentication fails, return appropriate response
            Response.StatusCode = 100;
            return Unauthorized("Invalid username or password");
        }     

    }
}
