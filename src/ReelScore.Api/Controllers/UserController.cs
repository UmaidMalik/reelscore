using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReelScore.Api;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Services;

namespace ReelScore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long userId)
        {
            return Ok(await _userService.GetUserById(userId));
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> PutUser(long id, [FromBody] UserDto userDto)
        {
            User user = new User
            {
                UserId = id,
                Username = userDto.Username,
                Email = userDto.Email
            };
            return Ok(await _userService.UpdateUser(id, user));
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] UserDto userDto)
        {
            User user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email
            };
            return Ok(await _userService.AddUser(user));
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            return Ok(await _userService.DeleteUser(id));
        }

        private async Task<bool> UserExists(long id)
        {
            return await _userService.UserExists(id);
        }
    }
}
