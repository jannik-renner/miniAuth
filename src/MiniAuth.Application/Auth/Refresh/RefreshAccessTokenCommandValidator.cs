using FluentValidation;

namespace MiniAuth.Application.Auth.Refresh
{
    public sealed class RefreshAccessTokenCommandValidator : AbstractValidator<RefreshAccessTokenCommand>
    {
        public RefreshAccessTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
