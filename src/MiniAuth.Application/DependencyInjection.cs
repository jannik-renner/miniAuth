using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MiniAuth.Application.Auth.Register;

namespace MiniAuth.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();

            services.AddScoped<RegisterUserService>();

            return services;
        }
    }
}
