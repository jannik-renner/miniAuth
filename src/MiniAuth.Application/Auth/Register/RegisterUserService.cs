using MiniAuth.Application.Abstractions;
using MiniAuth.Domain.Entities;
using MiniAuth.Application.Common.Exceptions;

namespace MiniAuth.Application.Auth.Register
{
    public class RegisterUserService
    {
        private const string ROLE_USER = "User";

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserService(IUserRepository userRepository, IRoleRepository roleRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (existingUser != null)
            {
                throw new ConflictException("A user with this email already exists.");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User(email, passwordHash);

            var role = await _roleRepository.GetByNameAsync(ROLE_USER, cancellationToken);

            if (role == null)
            {
                throw new InvalidOperationException("Default User role does not exist.");
            }

            user.UserRoles.Add(new UserRole(user.Id, role.Id));

            await _userRepository.AddAsync(user, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterUserResponse
            {
                UserId = user.Id,
                Email = user.Email
            };
        }
    }
}
