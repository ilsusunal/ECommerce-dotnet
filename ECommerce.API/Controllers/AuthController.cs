using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const string FakeUsername = "admin";
        private const string FakePassword = "1234";
        private const string FakeToken = "FAKE-TOKEN-123";

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == FakeUsername && request.Password == FakePassword)
            {
                return Ok(new { token = FakeToken });
            }

            return Unauthorized(new { message = "Invalid credentials" });
        }
    }
}
