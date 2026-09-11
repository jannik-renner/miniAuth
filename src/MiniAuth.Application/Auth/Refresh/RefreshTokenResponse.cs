
namespace MiniAuth.Application.Auth.Refresh
{
    public sealed class RefreshTokenResponse
    {
        public string AccessToken { get; init; } = null!;

        public string RefreshToken { get; init; } = null!;

        public DateTime AccessTokenExpiresAt { get; init; }

        public DateTime RefreshTokenExpiresAt { get; init; }
    }
}
