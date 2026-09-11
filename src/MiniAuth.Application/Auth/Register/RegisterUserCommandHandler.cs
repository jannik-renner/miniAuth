using MediatR;
using MiniAuth.Application.Abstractions;
using MiniAuth.Application.Common.Exceptions;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Application.Auth.Register
{
    public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (existingUser != null)
            {
                throw new ConflictException("A user with this email already exists.");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User(email, passwordHash);

            var role = await _roleRepository.GetByNameAsync("User", cancellationToken);

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