using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Net10.UserManagement.Domain.Repositories;
using Net10.UserManagement.Infrastructure.Repositories;

namespace Net10.UserManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
