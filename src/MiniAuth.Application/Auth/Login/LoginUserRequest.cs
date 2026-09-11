
namespace MiniAuth.Application.Auth.Login
{
    public class LoginUserRequest
    {
        public string Email { get; init; } = null!;

        public string Password { get; init; } = null!;
    }
}
