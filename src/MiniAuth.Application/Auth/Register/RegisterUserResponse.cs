
namespace MiniAuth.Application.Auth.Register
{
    public sealed class RegisterUserResponse
    {
        public Guid UserId { get; init; }

        public string Email { get; init; } = null!;
    }
}
