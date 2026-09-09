
namespace MiniAuth.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }

        public string Email { get; private set; } = null!;

        public string PasswordHash { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public bool IsActive { get; private set; }

        private User()
        {
        }

        public User(string email, string passwordHash)
        {
            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
