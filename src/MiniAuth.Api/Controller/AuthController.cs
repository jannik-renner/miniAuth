using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniAuth.Application.Auth.Login;
using MiniAuth.Application.Auth.Refresh;
using MiniAuth.Application.Auth.Register;

namespace MiniAuth.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserResponse>> Register(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(request.Email, request.Password);

            var result = await _sender.Send(command, cancellationToken);

            return Created($"/api/users/{result.UserId}", result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginUserResponse>> Login(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(request.Email, request.Password);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshTokenResponse>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var command = new RefreshAccessTokenCommand(request.RefreshToken);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
