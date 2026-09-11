using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniAuth.Application.Auth.Register;

namespace MiniAuth.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IValidator<RegisterUserRequest> _validator;

        public AuthController(ISender sender, IValidator<RegisterUserRequest> validator)
        {
            _sender = sender;
            _validator = validator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserResponse>> Register(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var command = new RegisterUserCommand(request.Email, request.Password);

            var result = await _sender.Send(command, cancellationToken);

            return Created($"/api/users/{result.UserId}", result);
        }
    }
}
