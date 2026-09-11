
namespace MiniAuth.Application.Abstractions
{
    public interface ITokenService
    {
        string CreateAccessToken(Guid userId, string email, IEnumerable<string> roles);
        string CreateRefreshToken();
        DateTime GetAccessTokenExpiresAt();
        DateTime GetRefreshTokenExpiresAt();
    }
}
