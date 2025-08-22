using Arbitrage.User.Application.Contracts;
using Arbitrage.User.Presentation.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arbitrage.User.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var user = await _userService.RegisterAsync(request.Username, request.Password, request.Email, request.TelegramId, request.Role);
                return Ok(new { user.Id, user.Username, user.Email, user.Role, user.CreatedAt });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var token = await _userService.LoginAsync(request.Username, request.Password);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users.Select(u => new { u.Id, u.Username, u.Role, u.CreatedAt, u.LastActive }));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();
                return Ok(new { user.Id, user.Username, user.Role, user.CreatedAt, user.LastActive });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequestDto request)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, request.Username, request.Role);
                return Ok(new { user.Id, user.Username, user.Role, user.CreatedAt });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}