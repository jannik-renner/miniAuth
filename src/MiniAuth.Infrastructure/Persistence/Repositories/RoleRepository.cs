using Microsoft.EntityFrameworkCore;
using MiniAuth.Application.Abstractions;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly MiniAuthDbContext _dbContext;

        public RoleRepository(MiniAuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
        }
    }
}
