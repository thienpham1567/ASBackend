using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
           _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult getAllUsers()
        {
            try
            {
                return Ok(_userRepository.GetAllUsers());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult getUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult getAllUsers(int id)
        {
            try
            {
                var user = _userRepository.DeleteUser(id);
                if (user != null)
                {
                    return StatusCode(StatusCodes.Status200OK, user);
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }
    }
}