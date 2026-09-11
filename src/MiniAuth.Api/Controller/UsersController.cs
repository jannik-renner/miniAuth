using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniAuth.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var email = User.FindFirstValue(ClaimTypes.Email);

            var roles = User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList();

            return Ok(new
            {
                userId,
                email,
                roles
            });
        }
    }
}
