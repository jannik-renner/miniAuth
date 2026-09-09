using MiniAuth.Domain.Entities;

namespace MiniAuth.Application.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task AddAsync(User user, CancellationToken cancellationToken = default);
    }
}
