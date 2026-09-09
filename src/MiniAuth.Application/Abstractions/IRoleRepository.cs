using MiniAuth.Domain.Entities;

namespace MiniAuth.Application.Abstractions
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
