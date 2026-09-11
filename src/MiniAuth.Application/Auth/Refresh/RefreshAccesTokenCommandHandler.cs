using MediatR;
using MiniAuth.Application.Abstractions;
using MiniAuth.Application.Common.Exceptions;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Application.Auth.Refresh
{
    public sealed class RefreshAccessTokenCommandHandler : IRequestHandler<RefreshAccessTokenCommand, RefreshTokenResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenHasher _tokenHasher;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshAccessTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, ITokenHasher tokenHasher, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenHasher = tokenHasher;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshAccessTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenHash = _tokenHasher.Hash(request.RefreshToken);

            var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

            if (existingToken == null || !existingToken.IsActive)
            {
                throw new UnauthorizedException("Invalid refresh token.");
            }

            var user = existingToken.User;

            if (!user.IsActive)
            {
                throw new UnauthorizedException("Invalid refresh token.");
            }

            var roles = user.UserRoles
                .Select(x => x.Role.Name)
                .ToList();

            // Revoke the token that was just used.
            existingToken.Revoke();

            // Generate a completely new token pair.
            var accessToken = _tokenService.CreateAccessToken(user.Id, user.Email, roles);

            var refreshToken = _tokenService.CreateRefreshToken();

            var refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiresAt();

            var newRefreshToken = new RefreshToken(user.Id, _tokenHasher.Hash(refreshToken), refreshTokenExpiresAt);

            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RefreshTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = _tokenService.GetAccessTokenExpiresAt(),
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }
    }
}
