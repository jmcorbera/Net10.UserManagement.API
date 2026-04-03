using Net10.UserManagement.Application;
using Net10.UserManagement.Infrastructure;

namespace Net10.UserManagement.Api.Bootstrapper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigServiceCollection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.AddWeb();
        return services;
    }
}
