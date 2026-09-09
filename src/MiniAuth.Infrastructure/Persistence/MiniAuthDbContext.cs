using Microsoft.EntityFrameworkCore;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Infrastructure.Persistence
{
    public class MiniAuthDbContext : DbContext
    {
        public MiniAuthDbContext(DbContextOptions<MiniAuthDbContext> options)
            : base(options)
        {
        }

#region DB Set
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        #endregion

#region Model Creation
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.Property(x => x.IsActive)
                    .IsRequired();

                entity.HasMany(x => x.UserRoles)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId);

                entity.HasMany(x => x.RefreshTokens)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });

            var userRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var adminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            modelBuilder.Entity<Role>().HasData(
                new
                {
                    Id = userRoleId,
                    Name = "User"
                },
                new
                {
                    Id = adminRoleId,
                    Name = "Admin"
                });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.UserId,
                    x.RoleId
                });

                entity.HasOne(x => x.User)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.UserId);

                entity.HasOne(x => x.Role)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.RoleId);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.TokenHash)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasIndex(x => x.TokenHash)
                    .IsUnique();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.Property(x => x.ExpiresAt)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.RefreshTokens)
                    .HasForeignKey(x => x.UserId);
            });
        }
#endregion
    }
}
