using DBApplication.Data;
using DBApplication.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DBApplication.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ApiDBContext _dbContext;
        public AuthController(ApiDBContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        private bool isValid(string username, string password)
        {
            var regex = new Regex("^[a-zA-Z0-9_]*$");
            var username_flag = regex.IsMatch(username.Trim()) && username.Trim().Length == 10;
            regex = new Regex("^(?=.*[a-z])(?=."
                    + "*[A-Z])(?=.*\\d)"
                    + "(?=.*[-+_!@#$%^&*., ?]).+$");
            var password_flag = regex.IsMatch(password.Trim()) && password.Trim().Length == 8;

            return username_flag && password_flag;
        }


        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> LoginUsers(LoginAuthData loginAuthData)
        {
            var user = await _dbContext.Users.Where(x => x.UserName == loginAuthData.UserName
                                                    && x.Password == loginAuthData.Password).FirstOrDefaultAsync();
            if (user != null && isValid(loginAuthData.UserName, loginAuthData.Password))
            {
                return Ok(user);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("/signup")]
        public async Task<IActionResult> AddUser(AddUserAuthData addUserAuthData)
        {
            var user = await _dbContext.Users.Where
                (x => x.UserName == addUserAuthData.UserName
                || x.Email == addUserAuthData.Email
                || x.PhoneNumber == addUserAuthData.PhoneNumber)
                .FirstOrDefaultAsync();
            
            if(user == null && isValid(addUserAuthData.UserName, addUserAuthData.Password))
            {
                var auth = new Auth()
                {
                    Uid = Guid.NewGuid(),
                    UserName = addUserAuthData.UserName,
                    Password = addUserAuthData.Password,
                    isAdmin = false,
                    FirstName = addUserAuthData.FirstName,
                    LastName = addUserAuthData.LastName,
                    Email = addUserAuthData.Email,
                    PhoneNumber = addUserAuthData.PhoneNumber
                };

                await _dbContext.Users.AddAsync(auth);
                await _dbContext.SaveChangesAsync();

                return Ok(auth);
            }

            return NotFound();
        }
    }
}
