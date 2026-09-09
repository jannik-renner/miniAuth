using Microsoft.AspNetCore.Mvc;

namespace MiniAuth.Api.Controller
{

    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "ok"
            });
        }
    }
}
