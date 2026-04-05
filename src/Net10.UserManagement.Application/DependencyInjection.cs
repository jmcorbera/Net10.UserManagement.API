using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Net10.UserManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddAutoMapper(cfg => {}, Assembly.GetExecutingAssembly());
        
        return services;
    }
}