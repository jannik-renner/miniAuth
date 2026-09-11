using MediatR;

namespace MiniAuth.Application.Auth.Register
{
    public sealed record RegisterUserCommand(string Email, string Password) : IRequest<RegisterUserResponse>;
}
