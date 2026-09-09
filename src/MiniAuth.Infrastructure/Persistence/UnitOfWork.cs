using MiniAuth.Application.Abstractions;
namespace MiniAuth.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MiniAuthDbContext _dbContext;

        public UnitOfWork(MiniAuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
