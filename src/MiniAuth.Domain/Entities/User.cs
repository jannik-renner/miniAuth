
namespace MiniAuth.Domain.Entities
{
    public class User
    {
        public const string ROLE_USER = "User";
        public const string ROLE_ADMIN = "Admin";


        public Guid Id { get; private set; }

        public string Email { get; private set; } = null!;

        public string PasswordHash { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public bool IsActive { get; private set; }

        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

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
