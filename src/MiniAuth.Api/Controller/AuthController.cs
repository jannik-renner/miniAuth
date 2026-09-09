using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MiniAuth.Application.Auth.Register;
using MiniAuth.Application.Common.Exceptions;

namespace MiniAuth.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RegisterUserService _registerUserService;
        private readonly IValidator<RegisterUserRequest> _validator;

        public AuthController(RegisterUserService registerUserService, IValidator<RegisterUserRequest> validator)
        {
            _registerUserService = registerUserService;
            _validator = validator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserResponse>> Register(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var result = await _registerUserService.RegisterAsync(request, cancellationToken);
                return Created($"/api/users/{result.UserId}", result);
            }
            catch (ConflictException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
