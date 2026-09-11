using Microsoft.EntityFrameworkCore;
using MiniAuth.Application.Abstractions;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Infrastructure.Persistence.Repositories
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly MiniAuthDbContext _dbContext;

        public RefreshTokenRepository(MiniAuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshTokens
                .Include(x => x.User)
                .ThenInclude(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        }
    }
}
