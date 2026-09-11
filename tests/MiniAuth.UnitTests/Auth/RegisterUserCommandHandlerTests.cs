using MiniAuth.Application.Abstractions;
using MiniAuth.Application.Auth.Register;
using MiniAuth.Application.Common.Exceptions;
using MiniAuth.Domain.Entities;

namespace MiniAuth.UnitTests.Auth
{
    public class RegisterUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldRegisterNewUser()
        {
            // Arrange
            var userRepository = new FakeUserRepository();
            var roleRepository = new FakeRoleRepository();
            var passwordHasher = new FakePasswordHasher();
            var unitOfWork = new FakeUnitOfWork();

            var userRole = new Role("User");
            roleRepository.Role = userRole;

            var handler = new RegisterUserCommandHandler(userRepository, roleRepository, passwordHasher, unitOfWork);

            var command = new RegisterUserCommand("TEST@EXAMPLE.COM", "MyPassword123!");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result.UserId);
            Assert.Equal("test@example.com", result.Email);

            var user = Assert.Single(userRepository.Users);

            Assert.Equal(result.UserId, user.Id);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("hashed-password", user.PasswordHash);

            var assignedRole = Assert.Single(user.UserRoles);

            Assert.Equal(user.Id, assignedRole.UserId);
            Assert.Equal(userRole.Id, assignedRole.RoleId);

            Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        }

        [Fact]
        public async Task Handle_ShouldRejectDuplicateEmail()
        {
            // Arrange
            var userRepository = new FakeUserRepository();
            var roleRepository = new FakeRoleRepository();
            var passwordHasher = new FakePasswordHasher();
            var unitOfWork = new FakeUnitOfWork();

            var existingUser = new User("test@example.com", "existing-hash");

            userRepository.Users.Add(existingUser);

            var handler = new RegisterUserCommandHandler(userRepository, roleRepository, passwordHasher, unitOfWork);

            var command = new RegisterUserCommand("TEST@EXAMPLE.COM", "MyPassword123!");

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(command, CancellationToken.None));

            Assert.Single(userRepository.Users);
            Assert.Equal(0, unitOfWork.SaveChangesCallCount);
        }

        [Fact]
        public async Task Handle_ShouldRejectRegistration_WhenUserRoleDoesNotExist()
        {
            // Arrange
            var userRepository = new FakeUserRepository();
            var roleRepository = new FakeRoleRepository();
            var passwordHasher = new FakePasswordHasher();
            var unitOfWork = new FakeUnitOfWork();

            // No "User" role configured.

            var handler = new RegisterUserCommandHandler(userRepository, roleRepository, passwordHasher, unitOfWork);

            var command = new RegisterUserCommand("test@example.com", "MyPassword123!");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

            Assert.Empty(userRepository.Users);
            Assert.Equal(0, unitOfWork.SaveChangesCallCount);
        }

        [Fact]
        public async Task Handle_ShouldHashPassword()
        {
            // Arrange
            var userRepository = new FakeUserRepository();
            var roleRepository = new FakeRoleRepository();
            var passwordHasher = new FakePasswordHasher();
            var unitOfWork = new FakeUnitOfWork();

            roleRepository.Role = new Role("User");

            var handler = new RegisterUserCommandHandler(userRepository, roleRepository, passwordHasher, unitOfWork);

            var command = new RegisterUserCommand("test@example.com", "MyPassword123!");

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("MyPassword123!", passwordHasher.PasswordReceived);
        }

        private sealed class FakeUserRepository : IUserRepository
        {
            public List<User> Users { get; } = new();

            public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            {
                var user = Users.SingleOrDefault( x => x.Email == email);

                return Task.FromResult(user);
            }

            public Task AddAsync(User user, CancellationToken cancellationToken = default)
            {
                Users.Add(user);

                return Task.CompletedTask;
            }
        }

        private sealed class FakeRoleRepository : IRoleRepository
        {
            public Role? Role { get; set; }

            public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
            {
                if (Role?.Name == name)
                {
                    return Task.FromResult<Role?>(Role);
                }

                return Task.FromResult<Role?>(null);
            }
        }

        private sealed class FakePasswordHasher : IPasswordHasher
        {
            public string? PasswordReceived { get; private set; }

            public string Hash(string password)
            {
                PasswordReceived = password;

                return "hashed-password";
            }

            public bool Verify(string password, string passwordHash)
            {
                return false;
            }
        }

        private sealed class FakeUnitOfWork : IUnitOfWork
        {
            public int SaveChangesCallCount { get; private set; }

            public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                SaveChangesCallCount++;

                return Task.FromResult(1);
            }
        }
    }
}
