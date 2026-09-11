using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MiniAuth.Application.Auth.Register;

namespace MiniAuth.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(
                    typeof(DependencyInjection).Assembly);
            });

            services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();

            return services;
        }
    }
}
