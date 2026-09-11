
namespace MiniAuth.Application.Auth.Login
{
    public class LoginUserResponse
    {
        public string AccessToken { get; init; } = null!;

        public string RefreshToken { get; init; } = null!;

        public DateTime AccessTokenExpiresAt { get; init; }

        public DateTime RefreshTokenExpiresAt { get; init; }
    }
}
