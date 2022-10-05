using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Services;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("MyCors")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITokenRepository _tokenRepository;

        public AccountController(AppDbContext context, IAccountRepository accountRepository, ITokenRepository tokenRepository)
        {
            _accountRepository = accountRepository;
            _tokenRepository = tokenRepository;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin model)
        {
            var user = _accountRepository.ValidateLogin(model);
            if (user == null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid user email or password",
                });
            }

            // generate token
            var token = await _tokenRepository.GenerateToken(user);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Login successfully",
                Data = new { token },
            });

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] User model)
        {
            var apiResponse = _accountRepository.ValidateRegister(model);
            if (apiResponse.Success)
            {
                return Ok(apiResponse);
            }
            return Ok(apiResponse);
        }

        [HttpPost("renewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var apiResponse = await _tokenRepository.ValidateToken(model);
            return Ok(apiResponse);
        }
    }
}