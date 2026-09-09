
namespace MiniAuth.Application.Auth.Register
{
    public sealed class RegisterUserRequest
    {
        public string Email { get; init; } = null!;

        public string Password { get; init; } = null!;
    }
}
