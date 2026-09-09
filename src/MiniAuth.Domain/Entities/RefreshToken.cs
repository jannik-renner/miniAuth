
namespace MiniAuth.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string TokenHash { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public DateTime ExpiresAt { get; private set; }

        public DateTime? RevokedAt { get; private set; }

        public User User { get; private set; } = null!;

        public bool IsRevoked => RevokedAt.HasValue;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public bool IsActive => !IsRevoked && !IsExpired;

        private RefreshToken()
        {
        }

        public RefreshToken(
            Guid userId,
            string tokenHash,
            DateTime expiresAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            TokenHash = tokenHash;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
        }

        public void Revoke()
        {
            RevokedAt = DateTime.UtcNow;
        }
    }
}
