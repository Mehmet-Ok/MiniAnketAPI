using Microsoft.AspNetCore.Mvc;
using MiniAnketDapper.Services;

namespace MiniAnketDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _authService.Login(request.Username, request.Password);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(new { Token = token });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
