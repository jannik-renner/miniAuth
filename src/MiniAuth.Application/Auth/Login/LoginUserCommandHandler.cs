using MediatR;
using MiniAuth.Application.Abstractions;
using MiniAuth.Application.Common.Exceptions;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Application.Auth.Login
{
    public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ITokenHasher _tokenHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService, ITokenHasher tokenHasher, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _tokenHasher = tokenHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var roles = user.UserRoles.Select(x => x.Role.Name).ToList();

            var accessToken = _tokenService.CreateAccessToken(user.Id, user.Email, roles);

            var refreshToken = _tokenService.CreateRefreshToken();

            var refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiresAt();

            var refreshTokenEntity = new RefreshToken(user.Id, _tokenHasher.Hash(refreshToken), refreshTokenExpiresAt);

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginUserResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = _tokenService.GetAccessTokenExpiresAt(),
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }
    }
}
