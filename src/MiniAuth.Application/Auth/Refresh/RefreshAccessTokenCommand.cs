using MediatR;

namespace MiniAuth.Application.Auth.Refresh
{
    public sealed record RefreshAccessTokenCommand(string RefreshToken) : IRequest<RefreshTokenResponse>;
}
