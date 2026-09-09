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

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.Property(x => x.IsActive)
                    .IsRequired();
            });
        }
    }
}
