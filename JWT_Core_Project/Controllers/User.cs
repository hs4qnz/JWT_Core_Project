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
    
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {

        private readonly ApplicationDbContext _dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        // GET: api/User
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            try
            {
                Console.WriteLine("Users: Get all users");
                if (_dbContext.users == null)
                {
                    return NotFound();
                }

                return await _dbContext.users.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Users: Get all users - ERROR: {0}", ex.Message.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.ToString());
            }
        }

        // GET: api/User/3
        [Authorize]
        [HttpGet("{user_id}")]
        public async Task<ActionResult<Users>> GetUsers(int user_id)
        {
            try
            {
                Console.WriteLine("User: Get One user (by User id)");

                if (_dbContext.users == null)
                {
                    return NotFound("The Users record couldn't be found.");
                }

                var user = await _dbContext.users.FindAsync(user_id);

                if (user == null)
                {
                    return NotFound("The Users record couldn't be found.");
                }

                return Ok(user);

            }
            catch (Exception ex)
            {
                Console.WriteLine("User: Get One User (by User id) - ERROR: {0}", ex.Message.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Users>> PostUsers(Users user)
        {
            try
            {
                Console.WriteLine("User: Create new user");

                // Check if the username already exists
                var existingUser = await _dbContext.users.FirstOrDefaultAsync(u => u.username == user.username);
                if (existingUser != null)
                {
                    return Conflict("Username already exists");
                }

                // Hash the user's password
                var passwordHasher = new PasswordHasher<Users>();
                user.password = passwordHasher.HashPassword(user, user.password);


                _dbContext.users.Add(user);
                await _dbContext.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("User: Create new user - ERROR: {0}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user");
            }
        }

        // DELETE: api/User/3
        [Authorize]
        [HttpDelete("{user_id}")]
        public async Task<ActionResult<Users>> DeleteUsers(int user_id)
        {
            try
            {
                Console.WriteLine("User: Delete One user (by User id)");

                if (_dbContext.users == null)
                {
                    return NotFound("The Users record couldn't be found.");
                }

                var user = await _dbContext.users.FindAsync(user_id);

                if (user == null)
                {
                    return NotFound("The Users record couldn't be found.");
                }

                _dbContext.users.Remove(user);
                await _dbContext.SaveChangesAsync();

                return Ok();


            }
            catch (Exception ex)
            {
                Console.WriteLine("User: Get One User (by User id) - ERROR: {0}", ex.Message.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.ToString());
            }
        }

        [Authorize]
        [HttpPut("{user_id}")]
        public async Task<IActionResult> PutUsers(int user_id, [FromBody] Users userUpdate)
        {
            try
            {
                Console.WriteLine("User: Update One user (by User id)");

                var user = await _dbContext.users.FindAsync(user_id);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Check if the updated username already exists in the database
                var existingUserWithSameUsername = await _dbContext.users.FirstOrDefaultAsync(u => u.username == userUpdate.username && u.user_id != user_id);
                if (existingUserWithSameUsername != null)
                {
                    return BadRequest("Username already exists.");
                }

                user.first_name = userUpdate.first_name;
                user.last_name = userUpdate.last_name;
                user.username = userUpdate.username;
                // user.updated_at = userUpdate.updated_at; 

                if (userUpdate.updated_by == null)
                {
                    user.updated_at = null;
                }
                else
                {
                    user.updated_at = DateTime.Now; //TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
                }

                // user.updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
                user.updated_by = userUpdate.updated_by;
                user.isActive = userUpdate.isActive;
                user.role_id = userUpdate.role_id;

                if (userUpdate.password != null)
                {
                    // Hash the user's new password
                    var passwordHasher = new PasswordHasher<Users>();
                    userUpdate.password = passwordHasher.HashPassword(userUpdate, userUpdate.password);

                    // Update the user's password in the database
                    user.password = userUpdate.password;
                }

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return NoContent();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Error updating user {user_id}: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user {user_id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the user.");
            }
        }




    }
}
