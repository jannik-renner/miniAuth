using MediatR;

namespace MiniAuth.Application.Auth.Login
{
    public sealed record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResponse>;
}
