using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MiniAuth.Infrastructure.Persistence;
using MiniAuth.Application.Abstractions;
using MiniAuth.Infrastructure.Persistence.Repositories;
using MiniAuth.Infrastructure.Security;

namespace MiniAuth.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            services.AddDbContext<MiniAuthDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITokenHasher, TokenHasher>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
